#
#
#
#
#
import os
from sys import argv
from XcodePBXProj import XcodeProject

xcodeProjPath = argv[1]
sdkPath = argv[2]
 
print('------- start ------')
print('Xcode build path --> ' + xcodeProjPath)
print('third sdk files path --> ' + sdkPath)

print('Step 1: open project and add system libraries')
project = XcodeProject.Load(xcodeProjPath + '/Unity-iPhone.xcodeproj/project.pbxproj')

# 
project.add_file_if_doesnt_exist('System/Library/Frameworks/Security.framework', parent=project.get_or_create_group('Frameworks'), tree='SDKROOT')
project.add_file_if_doesnt_exist('System/Library/Frameworks/SystemConfiguration.framework', parent=project.get_or_create_group('Frameworks'), tree='SDKROOT')
project.add_file_if_doesnt_exist('usr/lib/libz.dylib', parent=project.get_or_create_group('Frameworks'), tree='SDKROOT');

print('Step 2: ')
filesInSDK = os.listdir(sdkPath)
for f in filesInSDK:
	if not f.startswith('.'): #ignore .DS_STORE
		print f
		pathName = os.path.join(sdkPath, f)
		fileName, fileExtension = os.path.splitext(pathName)
		if not fileExtension == '.meta':
			if os.path.isfile(pathName):
				project.add_file(pathName, parent=project.get_or_create_group('Classes'))
			if os.path.isdir(pathName):
				project.add_folder(pathName, parent=project.get_or_create_group('Libraries'),excludes=["^*\.meta$"])


print('Step 3: ')


print('Step 4: modify the build setting')
project.add_other_buildsetting('GCC_ENABLE_OBJC_EXCEPTIONS', 'YES')

if project.modified:
  project.backup()
  project.saveFormat3_2()

print('------- end ------')