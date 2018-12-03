using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Engine.Infrastructure.Utils
{
    /// <summary>
    /// 枚举辅助类
    /// </summary> 
    public static class EnumHelper
    {
        #region-----共有静态方法-----

        /// <summary>
        /// 使用：
        ///     typeOf(IsEnable).GetEnumList();
        /// 获取枚举的Id和Name，包含枚举中的所有枚举项(不包括空项)
        /// </summary>
        /// <param name="enumType">枚举类型(需要查看的枚举类型typeof())</param>
        /// <returns></returns>
        public static IList<BaseEnumInf> GetEnumListInfo(this Type enumType)
        {
            if (!enumType.IsEnum) //首先判断传递进来的对象是否是枚举
            {
                throw new InvalidOperationException("传递的对象不是枚举项，抛出异常");
            }
            IList<BaseEnumInf> listBaseEnumInfs = new List<BaseEnumInf>();
            //获取枚举的Description特性(说明项)
            Type typeDescription = typeof (DescriptionAttribute);
            //获取所有的枚举字段
            FieldInfo[] fieldInfos = enumType.GetFields();
            foreach (var fieldInfo in fieldInfos) //循环得到的枚举对象
            {
                BaseEnumInf baseEnumInf = new BaseEnumInf();
                if (!fieldInfo.FieldType.IsEnum) //判断字段类型是否是枚举
                    continue;
                //获取枚举的Id信息
                baseEnumInf.Id = (int) enumType.InvokeMember(fieldInfo.Name, BindingFlags.GetField, null, null, null);
                //不包括空值
                if (baseEnumInf.Id >= 0)
                {
                    object[] arrayObjects = fieldInfo.GetCustomAttributes(typeDescription, false);
                    baseEnumInf.Name = arrayObjects.Length > 0
                        ? ((DescriptionAttribute) arrayObjects[0]).Description
                        : fieldInfo.Name;
                    //添加到列表中返回
                    listBaseEnumInfs.Add(baseEnumInf);
                }
            }
            return listBaseEnumInfs;
        }

        /// <summary>
        /// 使用：
        ///     CName=EnumHelper.GetEnumDesc(typeof(IsEnable),m.CType);
        /// 根据枚举值信息获取对象的Description属性
        /// </summary>
        /// <param name="enumType">枚举类型(需要得到的枚举类型)</param>
        /// <param name="id">查看描述的枚举的Id</param>
        /// <returns></returns>
        public static string GetEnumDescInfo(Type enumType, int? id)
        {
            Type typeDescription = typeof (DescriptionAttribute);
            FieldInfo[] fieldInfos = enumType.GetFields();
            //循环获得查询到的枚举信息
            foreach (var fieldInfo in fieldInfos)
            {
                if (fieldInfo.FieldType.IsEnum)
                {
                    if (
                        Convert.ToInt16(enumType.InvokeMember(fieldInfo.Name, BindingFlags.GetField, null, null, null)) ==
                        id)
                    {
                        object[] arrayObjects = fieldInfo.GetCustomAttributes(typeDescription, true);
                        if (arrayObjects.Length > 0)
                        {
                            var descriptionAttribute = (DescriptionAttribute) arrayObjects[0];
                            return descriptionAttribute.Description;
                        }
                    }
                }
            }
            return "";
        }

        /// <summary>
        /// 获取枚举变量值的Description属性
        /// </summary>
        /// <param name="obj">枚举变量</param>
        /// <param name="isTop"></param>
        /// <returns></returns>
        public static string GetDescriptionInfo(this object obj, bool isTop)
        {
            if (obj == null)
            {
                return string.Empty;
            }
            try
            {
                Type enumType = obj.GetType(); //获取当前实例的Type属性
                DescriptionAttribute description = null;
                if (isTop)
                {
                    description =
                        (DescriptionAttribute) Attribute.GetCustomAttribute(enumType, typeof (DescriptionAttribute));
                }
                else
                {
                    FieldInfo fieldInfo = enumType.GetField(Enum.GetName(enumType, obj));
                    description =
                        (DescriptionAttribute) Attribute.GetCustomAttribute(fieldInfo, typeof (DescriptionAttribute));
                }
                if (description != null && string.IsNullOrEmpty(description.Description) == false)
                {
                    return description.Description;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return obj.ToString();
        }

        #endregion
    }

    /// <summary>
    /// 定义类来存放枚举信息
    /// </summary>
    public class BaseEnumInf
    {
        /// <summary>
        /// 枚举Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 枚举名称
        /// </summary>
        public string Name { get; set; }
    }
}