
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Cherry.AssetBundlePacker
{
    /// <summary>
    /// 
    /// </summary>
    public class AssetBundleBuildData
    {
        /// <summary>
        ///   Asset's build data
        /// </summary>
        public class AssetBuild
        {
            /// <summary>
            ///   资源结点
            /// </summary>
            public class Element
            {
                /// <summary>
                ///   名称
                /// </summary>
                public string Name;
                /// <summary>
                ///   是否文件夹
                /// </summary>
                public bool IsFolder;
                /// <summary>
                ///   规则
                /// </summary>
                public int Rule;
                /// <summary>
                /// 是否压缩
                /// </summary>
                public bool IsCompress;
                /// <summary>
                /// 是否打包到安装包中
                /// </summary>
                public bool IsNative;
                /// <summary>
                /// 是否常驻内存
                /// </summary>
                public bool IsPermanent;
                /// <summary>
                /// 启动时加载
                /// </summary>
                public bool IsStartupLoad;
                /// <summary>
                ///   子对象
                /// </summary>
                public List<Element> Children;

                /// <summary>
                /// 
                /// </summary>
                public Element()
                {
                }

                /// <summary>
                /// 
                /// </summary>
                public Element(string name)
                {
                    Name = name;
                }

                /// <summary>
                ///   增加一个子对象
                /// </summary>
                public void Add(Element child)
                {
                    if (Children == null)
                        Children = new List<Element>();

                    Children.Add(child);
                }

                /// <summary>
                ///   查找文件夹
                /// </summary>
                public Element FindFolderElement(string name)
                {
                    if (Children == null)
                        return null;
                    return Children.Find((elem) =>
                    {
                        return elem.Name == name && elem.IsFolder;
                    });
                }

                /// <summary>
                ///   查找文件
                /// </summary>
                public Element FindFileElement(string name)
                {
                    if (Children == null)
                        return null;
                    return Children.Find((elem) =>
                    {
                        return elem.Name == name && !elem.IsFolder;
                    });
                }

                /// <summary>
                ///   子数量
                /// </summary>
                public int Count()
                {
                    int count = 0;
                    if (Children != null)
                    {
                        count += Children.Count;
                        for (int i = 0; i < Children.Count; ++i)
                        {
                            count += Children[i].Count();
                        }
                    }

                    return count;
                }

                /// <summary>
                /// 拷贝
                /// </summary>
                public void CopyTo(Element elem)
                {
                    elem.Name = Name;
                    elem.IsFolder = IsFolder;
                    elem.Rule = Rule;
                    elem.IsCompress = IsCompress;
                    elem.IsNative = IsNative;
                    elem.IsPermanent = IsPermanent;
                    elem.Children = new List<Element>(Children);
                }

                /// <summary>
                ///   
                /// </summary>
                public override bool Equals(object obj)
                {
                    if (obj == null)
                    {
                        return false;
                    }
                    if (obj.GetType() != this.GetType())
                    {
                        return false;
                    }

                    Element other = obj as Element;
                    if (this.Name != other.Name)
                        return false;
                    if (this.IsFolder != other.IsFolder)
                        return false;
                    if (this.Rule != other.Rule)
                        return false;
                    if (this.Children == null && other.Children != null)
                        return false;
                    if (this.Children != null && other.Children == null)
                        return false;
                    if (this.Children != null && other.Children != null)
                    {
                        if (this.Children.Count != other.Children.Count)
                            return false;

                        int count = this.Children.Count;
                        for (int i = 0; i < count; ++i)
                        {
                            if (!this.Children[i].Equals(other.Children[i]))
                                return false;
                        }
                    }

                    return true;
                }

                /// <summary>
                /// 
                /// </summary>
                public override int GetHashCode()
                {
                    return Name.GetHashCode();
                }

                /// <summary>
                /// 排序
                /// 1.优先显示文件夹(以字符顺序排序)
                /// 2.其次显示文件(以字符顺序排序)
                /// </summary>
                public void SortChildren()
                {
                    if(Children != null && Children.Count > 1)
                    {
                        Children.Sort(_ComparisonElement);
                    }
                }

                int _ComparisonElement(Element x, Element y)
                {
                    if((x.IsFolder && y.IsFolder) || (!x.IsFolder && !y.IsFolder))
                    {
                        return string.Compare(x.Name, y.Name, StringComparison.Ordinal);
                    }
                    else if(x.IsFolder)
                    {
                        return -1;
                    }
                    else if (y.IsFolder)
                    {
                        return 1;
                    }

                    return -1;
                }
            }

            public Element Root;
        }

        /// <summary>
        ///   Scene's build data
        /// </summary>
        public class SceneBuild
        {
            public class Element
            {
                /// <summary>
                /// 场景路径
                /// </summary>
                public string ScenePath;
                /// <summary>
                /// 是否打包
                /// </summary>
                public bool IsBuild;
                /// <summary>
                /// 是否压缩
                /// </summary>
                public bool IsCompress;
                /// <summary>
                /// 是否打包到安装包中
                /// </summary>
                public bool IsNative;
            }
            public List<Element> Scenes = new List<Element>();
        }

        /// <summary>
        /// 版本号
        /// </summary>
        public string strVersion;

        /// <summary>
        /// AssetBundle打包起始相对路径
        /// </summary>
        public string BuildStartLocalPath = Common.PROJECT_ASSET_ROOT_NAME;

        /// <summary>
        /// 是否打包所有AssetBundle至安装包
        /// </summary>
        public bool IsAllNative;

        /// <summary>
        /// 是否所有AssetBundle都压缩
        /// </summary>
        public bool IsAllCompress;

        /// <summary>
        /// 资源
        /// </summary>
        public AssetBuild Assets = new AssetBuild();

        /// <summary>
        /// 场景
        /// </summary>
        public SceneBuild Scenes = new SceneBuild();
    }

}

