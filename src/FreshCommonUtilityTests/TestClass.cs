using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreshCommonUtility.DeepCopy;

namespace FreshCommonUtilityTests
{
    public class TestClass
    {
        /// <summary>
        /// Test list set.
        /// </summary>
        private readonly List<TestsTabelToListObject> _testList = new List<TestsTabelToListObject>
        {
            new TestsTabelToListObject
            {
                Age = 10,
                Height = 20.907,
                Name = "qinxianbo",
                Right = true,
                Sex = EnumSex.boy,
                YouLong = new TimeSpan(1, 1, 1, 1)
            },
            new TestsTabelToListObject
            {
                Age = 23,
                Height = 234.907,
                Name = "秦先波",
                Right = true,
                Sex = EnumSex.boy,
                YouLong = new TimeSpan(1, 1, 1, 2)
            },
            new TestsTabelToListObject
            {
                Age = 40,
                Height = 20.907,
                Name = "qinxianbo",
                Right = true,
                Sex = EnumSex.boy,
                YouLong = new TimeSpan(1, 1, 1, 3)
            },
            new TestsTabelToListObject
            {
                Height = 20.907,
                Name = "杨宏俊",
                Right = true,
                Sex = EnumSex.grily,
                YouLong = new TimeSpan(1, 1, 1, 4)
            },
            new TestsTabelToListObject
            {
                Age = 10,
                Name = "k",
                Height = 20.907,
                Right = true,
                Sex = EnumSex.boy,
                YouLong = new TimeSpan(1, 1, 1, 5)
            }
        };

        #region [1、 Test deep copy helper]

        /// <summary>
        /// Test deep copy helper
        /// </summary>
        public void DeepCopyTest()
        {
            var tempObj = new TestsTabelToListObject
            {
                Age = 10,
                Name = "k",
                Height = 20.907,
                Right = true,
                Sex = EnumSex.boy,
                YouLong = new TimeSpan(1, 1, 1, 5),
                AdressList = new List<string> { "Chongqing", "Beijing", "Shanghai" }
            };
            var copyResult = tempObj.DeepCopy();
            new TimeSpan(1, 1, 1, 5).IsEqualTo(copyResult.YouLong);

            var list = new List<TestsTabelToListObject>();
            _testList.ForEach(f =>
            {
                var temp = new TestsTabelToListObject();
                f.DeepCopy(temp);
                list.Add(temp);
            });
            list.Count.IsEqualTo(_testList.Count);
        }

        /// <summary>
        /// RecursionTest deep copy test
        /// </summary>
        public void DeepCopyRecursionTest()
        {
            var tempObj = new TestsTabelToListObject
            {
                Age = 10,
                Name = "k",
                Height = 20.907,
                Right = true,
                Sex = EnumSex.boy,
                YouLong = new TimeSpan(1, 1, 1, 5),
                AdressList = new List<string> { "Chongqing", "Beijing", "Shanghai" }
            };
            var copyResult = DeepCopyHelper.DeepCopyRecursion(tempObj) as TestsTabelToListObject;
            new TimeSpan(1, 1, 1, 5).IsEqualTo(copyResult.YouLong);
            tempObj.AdressList[2] = "TianAnMen";
            tempObj.AdressList[2].IsNotEqualTo(copyResult.AdressList[2]);

            var list = new List<TestsTabelToListObject>();
            _testList.ForEach(f =>
            {
                var temp = new TestsTabelToListObject();
                temp = DeepCopyHelper.DeepCopyRecursion(f) as TestsTabelToListObject;
                list.Add(temp);
            });
            list.Count.IsEqualTo(_testList.Count);
        }
        #endregion
    }

    public class TestsTabelToListObject
    {
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        public int Age { get; set; }

        public double Height { get; set; }

        public EnumSex Sex { get; set; }

        public TimeSpan YouLong { get; set; }

        public bool Right { get; set; }

        public List<string> AdressList { get; set; }
    }

    public enum EnumSex
    {
        boy,
        grily
    }
}
