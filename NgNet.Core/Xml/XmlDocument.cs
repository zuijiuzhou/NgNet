﻿using System.Xml;

namespace NgNet.Xml
{
    /// <summary>
    /// Xml的操作公共类
    /// </summary>    
    public class XmlDocument
    {
        #region 字段定义
        /// <summary>
        /// XML文件的物理路径
        /// </summary>
        private string _filePath = string.Empty;
        /// <summary>
        /// Xml文档
        /// </summary>
        private System.Xml.XmlDocument _xml;
        /// <summary>
        /// XML的根节点
        /// </summary>
        private XmlElement _element;
        #endregion

        #region 构造方法
        /// <summary>
        /// 实例化XmlHelper对象
        /// </summary>
        /// <param name="xmlFilePath">Xml文件的相对路径</param>
        public XmlDocument(string xmlFilePath)
        {
            //获取XML文件的绝对路径
            _filePath = xmlFilePath;
        }
        #endregion

        #region 创建XML的根节点
        /// <summary>
        /// 创建XML的根节点
        /// </summary>
        private void CreateXMLElement()
        {
            //创建一个XML对象
            _xml = new System.Xml.XmlDocument();

            if (System.IO.File.Exists(_filePath))
            {
                //加载XML文件
                _xml.Load(this._filePath);
            }

            //为XML的根节点赋值
            _element = _xml.DocumentElement;
        }
        #endregion

        #region 获取指定XPath表达式的节点对象
        /// <summary>
        /// 获取指定XPath表达式的节点对象
        /// </summary>        
        /// <param name="xPath">XPath表达式,
        /// 范例1: @"Skill/First/SkillItem", 等效于 @"//Skill/First/SkillItem"
        /// 范例2: @"Table[USERNAME='a']" , []表示筛选,USERNAME是Table下的一个子节点.
        /// 范例3: @"ApplyPost/Item[@itemName='岗位编号']",@itemName是Item节点的属性.
        /// </param>
        public XmlNode GetNode(string xPath)
        {
            //创建XML的根节点
            CreateXMLElement();
            //返回XPath节点
            return _element.SelectSingleNode(xPath);
        }
        #endregion

        #region 获取指定XPath表达式节点的值
        /// <summary>
        /// 获取指定XPath表达式节点的值
        /// </summary>
        /// <param name="xPath">XPath表达式,
        /// 范例1: @"Skill/First/SkillItem", 等效于 @"//Skill/First/SkillItem"
        /// 范例2: @"Table[USERNAME='a']" , []表示筛选,USERNAME是Table下的一个子节点.
        /// 范例3: @"ApplyPost/Item[@itemName='岗位编号']",@itemName是Item节点的属性.
        /// </param>
        public string GetValue(string xPath)
        {
            //创建XML的根节点
            CreateXMLElement();

            //返回XPath节点的值
            return _element.SelectSingleNode(xPath).InnerText;
        }
        #endregion

        #region 获取指定XPath表达式节点的属性值
        /// <summary>
        /// 获取指定XPath表达式节点的属性值
        /// </summary>
        /// <param name="xPath">XPath表达式,
        /// 范例1: @"Skill/First/SkillItem", 等效于 @"//Skill/First/SkillItem"
        /// 范例2: @"Table[USERNAME='a']" , []表示筛选,USERNAME是Table下的一个子节点.
        /// 范例3: @"ApplyPost/Item[@itemName='岗位编号']",@itemName是Item节点的属性.
        /// </param>
        /// <param name="attributeName">属性名</param>
        public string GetAttributeValue(string xPath, string attributeName)
        {
            //创建XML的根节点
            CreateXMLElement();

            //返回XPath节点的属性值
            return _element.SelectSingleNode(xPath).Attributes[attributeName].Value;
        }
        #endregion

        #region 新增节点
        /// <summary>
        /// 1. 功能：新增节点。
        /// 2. 使用条件：将任意节点插入到当前Xml文件中。
        /// </summary>        
        /// <param name="xmlNode">要插入的Xml节点</param>
        public void AppendNode(XmlNode xmlNode)
        {
            //创建XML的根节点
            CreateXMLElement();
            //导入节点
            XmlNode node = _xml.ImportNode(xmlNode, true);
            //将节点插入到根节点下
            _element.AppendChild(node);
        }

        #endregion

        #region 删除节点
        /// <summary>
        /// 删除指定XPath表达式的节点
        /// </summary>        
        /// <param name="xPath">XPath表达式,
        /// 范例1: @"Skill/First/SkillItem", 等效于 @"//Skill/First/SkillItem"
        /// 范例2: @"Table[USERNAME='a']" , []表示筛选,USERNAME是Table下的一个子节点.
        /// 范例3: @"ApplyPost/Item[@itemName='岗位编号']",@itemName是Item节点的属性.
        /// </param>
        public void RemoveNode(string xPath)
        {
            //创建XML的根节点
            CreateXMLElement();

            //获取要删除的节点
            XmlNode node = _xml.SelectSingleNode(xPath);

            //删除节点
            _element.RemoveChild(node);
        }
        #endregion //删除节点

        #region 保存XML文件
        /// <summary>
        /// 保存XML文件
        /// </summary>        
        public void Save()
        {
            //创建XML的根节点
            CreateXMLElement();

            //保存XML文件
            _xml.Save(this._filePath);
        }
        #endregion //保存XML文件

        #region 静态方法

        #region 创建根节点对象
        /// <summary>
        /// 创建根节点对象
        /// </summary>
        /// <param name="xmlFilePath">Xml文件的相对路径</param>        
        private static XmlElement CreateRootElement(string xmlFilePath)
        {
            //定义变量，表示XML文件的绝对路径
            string filePath = "";

            //获取XML文件的绝对路径
            filePath = xmlFilePath;

            //创建XmlDocument对象
            System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
            //加载XML文件
            xmlDocument.Load(filePath);

            //返回根节点
            return xmlDocument.DocumentElement;
        }
        #endregion

        #region 获取指定XPath表达式节点的值
        /// <summary>
        /// 获取指定XPath表达式节点的值
        /// </summary>
        /// <param name="xmlFilePath">Xml文件的相对路径</param>
        /// <param name="xPath">XPath表达式,
        /// 范例1: @"Skill/First/SkillItem", 等效于 @"//Skill/First/SkillItem"
        /// 范例2: @"Table[USERNAME='a']" , []表示筛选,USERNAME是Table下的一个子节点.
        /// 范例3: @"ApplyPost/Item[@itemName='岗位编号']",@itemName是Item节点的属性.
        /// </param>
        public static string GetValue(string xmlFilePath, string xPath)
        {
            //创建根对象
            XmlElement rootElement = CreateRootElement(xmlFilePath);

            //返回XPath节点的值
            return rootElement.SelectSingleNode(xPath).InnerText;
        }
        #endregion

        #region 获取指定XPath表达式节点的属性值
        /// <summary>
        /// 获取指定XPath表达式节点的属性值
        /// </summary>
        /// <param name="xmlFilePath">Xml文件的相对路径</param>
        /// <param name="xPath">XPath表达式,
        /// 范例1: @"Skill/First/SkillItem", 等效于 @"//Skill/First/SkillItem"
        /// 范例2: @"Table[USERNAME='a']" , []表示筛选,USERNAME是Table下的一个子节点.
        /// 范例3: @"ApplyPost/Item[@itemName='岗位编号']",@itemName是Item节点的属性.
        /// </param>
        /// <param name="attributeName">属性名</param>
        public static string GetAttributeValue(string xmlFilePath, string xPath, string attributeName)
        {
            //创建根对象
            XmlElement rootElement = CreateRootElement(xmlFilePath);

            //返回XPath节点的属性值
            return rootElement.SelectSingleNode(xPath).Attributes[attributeName].Value;
        }
        #endregion

        #endregion

        public static void SetValue(string xmlFilePath, string xPath, string newtext)
        {
            //string path = SysHelper.GetPath(xmlFilePath);
            //var queryXML = from xmlLog in xelem.Descendants("msg_log")
            //               //所有名字为Bin的记录
            //               where xmlLog.Element("user").Value == "Bin"
            //               select xmlLog;

            //foreach (XElement el in queryXML)
            //{
            //    el.Element("user").Value = "LiuBin";//开始修改
            //}
            //xelem.Save(path);
        }
    }
}
