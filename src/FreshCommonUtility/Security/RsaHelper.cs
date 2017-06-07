using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace FreshCommonUtility.Security
{
    /// <summary>
    /// RSA helper.
    /// </summary>
    public class RsaHelper
    {
        /// <summary>
        /// openssl genrsa -out rsa_1024_priv.pem 1024
        /// </summary>
        private static readonly string PrivateKey = @"MIICXgIBAAKBgQCe5ibrbyz2FV9MEQ/vWqo7dFDSOr0Ud4MW+vWGISZM5ig/FUNB
ZgF/CvufNeecF0gcVse8VlwbsRVEU6YoCJ0IegA8TQ9asQtO9QO81kSYQDHafjQy
gh1B7XhteRKhczTMp4A48NnyT47Nn/DiCLi2WoIi4J0WJqpd5sjXvAmaPwIDAQAB
AoGBAIDWN+RZRmlZNuWkR/lY2AJQ818iBUFdDctKPF0+76EWVLehc+DI5pmtvAuB
V4P2VJ+1tUl99Dz3vjiiYmy/XyLCr7T6Xph8xwfTtP2ENo0WpKIyON08i+VfoFEu
cjq7tPPT7f6YQ8SHF/SIUtz0aaV3YoYKaLsYCyxPcfC3dQOhAkEAveJbAY/C1g8d
OVn7g82Aa4ZzzVJsWfTW3GScc4SvkSx+z8TGChQK98iWSU60DM/4guaR+ZD0wkNH
hf3ewxe4dwJBANY54b9V/lbgxPJ0froSucPqRHFov28slMBTVos6SRyMj299J3cs
5q77IBZNNaDX14PqZSTpb6fqdn8r9elMZnkCQGoOzSABKSUgygTnkokatjjYn0O2
Xtib3Yq6E3yeRuXCQY5Q7QBiE4I0omSNthlV7AtJN416fosmwwM/OjYjwJ8CQQDJ
5XE5h00WCe3zZxFMQnurBa2NiJ/qogRrId/NhZgD/QDtnPFF4x5hyTEbc5bYSLPH
km/SkuJ1SYZ2IjM3tZZZAkEAhWXW8kHe4qFcXFH9Y+WC5iQrOCA2anwxlvrriFhf
xemCEr6uxcwZFgpYqAz+F2U6R07H5G88KZQ9MSZb34GQ5g==".Replace("\n", "");

        /// <summary>
        /// openssl rsa -pubout -in rsa_1024_priv.pem -out rsa_1024_pub.pem
        /// </summary>
        private static readonly string PublicKey = @"MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCe5ibrbyz2FV9MEQ/vWqo7dFDS
Or0Ud4MW+vWGISZM5ig/FUNBZgF/CvufNeecF0gcVse8VlwbsRVEU6YoCJ0IegA8
TQ9asQtO9QO81kSYQDHafjQygh1B7XhteRKhczTMp4A48NnyT47Nn/DiCLi2WoIi
4J0WJqpd5sjXvAmaPwIDAQAB".Replace("\n", "");

        /// <summary>
        /// RSA En code
        /// </summary>
        /// <param name="encryptString">need encode string</param>
        /// <param name="enpublicKey">public key,if you want use you self key,please enter you key,suggestion you set the key have 2048bit if you must set youselft key.</param>
        /// <returns></returns>
        public static string RsaEncode(string encryptString, string enpublicKey=null)
        {
            var publicKey = string.IsNullOrEmpty(enpublicKey) ? PublicKey : enpublicKey;
            RSA rsa = CreateRsaFromPublicKey(publicKey);
            
            var plainTextBytes = Encoding.UTF8.GetBytes(encryptString);
            var cipherBytes = rsa.Encrypt(plainTextBytes, RSAEncryptionPadding.Pkcs1);
            var cipher = Convert.ToBase64String(cipherBytes);
            return cipher;
        }

        /// <summary>
        /// RSA Decode
        /// </summary>
        /// <param name="decryptString">need decode string</param>
        /// <param name="deprivateKey">private key,if you want use you self key,please enter you key,suggestion you set the key have 2048bit if you must set youselft key.</param>
        /// <returns></returns>
        public static string RsaDeCode(string decryptString, string deprivateKey = null)
        {
            var tempprivateKey = string.IsNullOrEmpty(deprivateKey) ? PrivateKey : deprivateKey;
            RSA rsa = CreateRsaFromPrivateKey(tempprivateKey);
            var cipherBytes = Convert.FromBase64String(decryptString);
            var plainTextBytes = rsa.Decrypt(cipherBytes, RSAEncryptionPadding.Pkcs1);
            var plainText = Encoding.UTF8.GetString(plainTextBytes);
            return plainText;
        }

        /// <summary>
        /// create RSA private key
        /// </summary>
        /// <param name="privateKey"></param>
        /// <returns></returns>
        private static RSA CreateRsaFromPrivateKey(string privateKey)
        {
            var privateKeyBits = Convert.FromBase64String(privateKey);
            var rsa = RSA.Create();
            var rsAparams = new RSAParameters();

            using (var binr = new BinaryReader(new MemoryStream(privateKeyBits)))
            {
                byte bt;
                ushort twobytes;
                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130)
                    binr.ReadByte();
                else if (twobytes == 0x8230)
                    binr.ReadInt16();
                else
                    throw new Exception("Unexpected value read binr.ReadUInt16()");

                twobytes = binr.ReadUInt16();
                if (twobytes != 0x0102)
                    throw new Exception("Unexpected version");

                bt = binr.ReadByte();
                if (bt != 0x00)
                    throw new Exception("Unexpected value read binr.ReadByte()");

                rsAparams.Modulus = binr.ReadBytes(GetIntegerSize(binr));
                rsAparams.Exponent = binr.ReadBytes(GetIntegerSize(binr));
                rsAparams.D = binr.ReadBytes(GetIntegerSize(binr));
                rsAparams.P = binr.ReadBytes(GetIntegerSize(binr));
                rsAparams.Q = binr.ReadBytes(GetIntegerSize(binr));
                rsAparams.DP = binr.ReadBytes(GetIntegerSize(binr));
                rsAparams.DQ = binr.ReadBytes(GetIntegerSize(binr));
                rsAparams.InverseQ = binr.ReadBytes(GetIntegerSize(binr));
            }

            rsa.ImportParameters(rsAparams);
            return rsa;
        }

        /// <summary>
        /// Get seize.
        /// </summary>
        /// <param name="binr"></param>
        /// <returns></returns>
        private static int GetIntegerSize(BinaryReader binr)
        {
            byte bt;
            int count;
            bt = binr.ReadByte();
            if (bt != 0x02)
                return 0;
            bt = binr.ReadByte();

            if (bt == 0x81)
                count = binr.ReadByte();
            else
                if (bt == 0x82)
            {
                var highbyte = binr.ReadByte();
                var lowbyte = binr.ReadByte();
                byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                count = BitConverter.ToInt32(modint, 0);
            }
            else
            {
                count = bt;
            }

            while (binr.ReadByte() == 0x00)
            {
                count -= 1;
            }
            binr.BaseStream.Seek(-1, SeekOrigin.Current);
            return count;
        }

        /// <summary>
        /// Create public key.
        /// </summary>
        /// <param name="publicKeyString"></param>
        /// <returns></returns>
        private static RSA CreateRsaFromPublicKey(string publicKeyString)
        {
            byte[] seqOid = { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };

            var x509Key = Convert.FromBase64String(publicKeyString);

            using (var mem = new MemoryStream(x509Key))
            {
                using (var binr = new BinaryReader(mem))
                {
                    byte bt;
                    ushort twobytes;

                    twobytes = binr.ReadUInt16();
                    if (twobytes == 0x8130)
                        binr.ReadByte();
                    else if (twobytes == 0x8230)
                        binr.ReadInt16();
                    else
                        return null;

                    var seq = binr.ReadBytes(15);
                    if (!CompareBytearrays(seq, seqOid))
                        return null;

                    twobytes = binr.ReadUInt16();
                    if (twobytes == 0x8103)
                        binr.ReadByte();
                    else if (twobytes == 0x8203)
                        binr.ReadInt16();
                    else
                        return null;

                    bt = binr.ReadByte();
                    if (bt != 0x00)
                        return null;

                    twobytes = binr.ReadUInt16();
                    if (twobytes == 0x8130)
                        binr.ReadByte();
                    else if (twobytes == 0x8230)
                        binr.ReadInt16();
                    else
                        return null;

                    twobytes = binr.ReadUInt16();
                    byte lowbyte;
                    byte highbyte = 0x00;

                    if (twobytes == 0x8102)
                        lowbyte = binr.ReadByte();
                    else if (twobytes == 0x8202)
                    {
                        highbyte = binr.ReadByte();
                        lowbyte = binr.ReadByte();
                    }
                    else
                        return null;
                    byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                    int modsize = BitConverter.ToInt32(modint, 0);

                    int firstbyte = binr.PeekChar();
                    if (firstbyte == 0x00)
                    {
                        binr.ReadByte();
                        modsize -= 1;
                    }

                    byte[] modulus = binr.ReadBytes(modsize);

                    if (binr.ReadByte() != 0x02)
                        return null;
                    int expbytes = binr.ReadByte();
                    byte[] exponent = binr.ReadBytes(expbytes);

                    var rsa = RSA.Create();
                    var rsaKeyInfo = new RSAParameters
                    {
                        Modulus = modulus,
                        Exponent = exponent
                    };
                    rsa.ImportParameters(rsaKeyInfo);
                    return rsa;
                }

            }
        }

        /// <summary>
        /// compare byte arry data.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static bool CompareBytearrays(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                return false;
            int i = 0;
            foreach (byte c in a)
            {
                if (c != b[i])
                    return false;
                i++;
            }
            return true;
        }
    }
}
