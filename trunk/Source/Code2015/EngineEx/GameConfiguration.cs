using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D.Config;
using Apoc3D.Vfs;
using System.Xml;

namespace Code2015.EngineEx
{
    /// <summary>
    ///  表示游戏xml配置文件的格式
    /// </summary>
    public class GameConfigurationFormat : ConfigurationFormat
    {
        /// <summary>
        ///  扩展名过滤器
        /// </summary>
        public override string[] Filters
        {
            get { return new string[] { ".xml" }; }
        }

        /// <summary>
        ///  从资源中读取游戏xml配置
        /// </summary>
        /// <param name="rl">表示资源的位置的<see cref="ResourceLocation"/></param>
        /// <returns>一个<see cref="Configuration"/>，表示创建好的配置</returns>
        public override Configuration Load(ResourceLocation rl)
        {
            return new GameConfiguration(rl);
        }
    }

    class GameConfiguration : Configuration
    {
        public static bool MoveToNextElement(XmlTextReader xmlIn)
        {
            if (!xmlIn.Read())
                return false;

            while (xmlIn.NodeType == XmlNodeType.EndElement)
            {
                if (!xmlIn.Read())
                    return false;
            }

            return true;
        }

        public GameConfiguration(ResourceLocation fl)
            : base(fl.Name, EqualityComparer<string>.Default)
        {
            XmlTextReader xml = new XmlTextReader(fl.GetStream);
            xml.WhitespaceHandling = WhitespaceHandling.None;

            int depth = xml.Depth;

            GameConfigurationSection currentSection = null;

            string currentAttrib = string.Empty;

            while (MoveToNextElement(xml))
            {
                switch (xml.NodeType)
                {
                    case XmlNodeType.Element:
                    case XmlNodeType.Text:
                    case XmlNodeType.CDATA:
                        switch (xml.Depth)
                        {
                            case 1:
                                currentSection = new GameConfigurationSection(xml.Name);
                                Add(xml.Name, currentSection);
                                break;
                            case 2:
                                currentAttrib = xml.Name;
                                break;
                            case 3:
                                currentSection.Add(currentAttrib, xml.ReadString());
                                break;
                        }
                        break;
                }
            }

            xml.Close();
        }

        public override Configuration Clone()
        {
            throw new NotImplementedException();
        }

        public override void Merge(Configuration config)
        {
            throw new NotImplementedException();
        }
    }
}
