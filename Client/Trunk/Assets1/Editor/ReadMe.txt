编辑器功能说明

1.AutoCreateUIScriptEditor.cs
  说明:自动生成UI界面脚本代码，使用方法点击UIPrefab，就会将代码生成到lua/文件夹下
2.CreateVersionManifestEditor
  说明:生成资源md5配置，并加密lua资源，复制到CDN待上传目录.
 

4.版本制作流程
  1.命令行输入版本号
  2.调用unity命令，开始打包
  3.修改项目版本号，开成生成bundle资源
  4.复制并加密lua资源
  5.生成资源清单，于服务器对比，将新增的存放到待CDN上传目录
  6.将资源拷贝到stringAssets目录
  7.生成版本日志文件
  9.开始打包，生成apk文件。
  