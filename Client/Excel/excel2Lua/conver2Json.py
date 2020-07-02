# -*- coding: utf-8 -*-
import importlib,sys
import xlrd
import pdb
import types
importlib.reload(sys)

inputDir = "F:/BaiduNetdiskDownload/Game/Excel/Excel/"

# 读取文件
def open_excel(file):
    try:
        data = xlrd.open_workbook(filename=file, encoding_override="utf-8")
        return data
    except Exception as e:
        print(str(e))


# 解析目录文件
# @param file 目录文件名
def open_contents(file, outputDir):
    data = open_excel(file)
    table = data.sheet_by_name("first")
    nrows = table.nrows

    cfgNameList = []

    for index in range(1, nrows):
        row = table.row_values(index)
        input = inputDir + row[0]
        sheet = row[1]
        out = row[2]
        luaName = row[3]
        # type = row[4]
        # startCol = converToInt(row[5])
        outputJson = converToInt(row[6])

        if outputJson == 0:
            continue

        tableData = open_excel(input)

        if sheet == "":
            curTable = tableData.sheet_by_index(0)
        else:
            curTable = tableData.sheet_by_name(sheet)

        # 计算哪些字段需要打进去
        allNeedFiledArr = []
        colnames = curTable.row_values(1)
        useTypeList = curTable.row_values(3)
        for colIndex in range(1, len(colnames)):
            fieldName = colnames[colIndex]
            useType = useTypeList[colIndex]
            if (fieldName != ""):
                if (useType == u"客户端" or useType == u"客户端+服务器"):
                    allNeedFiledArr.append(fieldName)

        # 没有需要的字段就不打表
        if (len(allNeedFiledArr) <= 0):
            continue

        print("Generate " + out + ".json from " + input)
        Generate_Json_code(out, luaName, curTable, outputDir)
        cfgNameList.append(luaName)

    # generate_tableManager(cfgNameList, outputDir)

# 生成json配置文件
def Generate_Json_code(fileName, luaName, table, outputDir):
    nrows = table.nrows  # 行数
    column = table.ncols # 列数

    colnames = table.row_values(1)
    file = open(outputDir + fileName+".json", "w", encoding='utf-8')
    file.write("{\n")
    colnames = table.row_values(1)
    typeList = table.row_values(2)
    useTypeList = table.row_values(3)

    for rowNum in range(4, nrows):
        row = table.row_values(rowNum)
        if len(row) != column:
            print("表格数据配置异常 名称:"+fileName+" 行号:"+rowNum)
            continue

        lineStr = ""
        idStr = ""
        for i in range(0, column):
            if CheckFieldFormat(useTypeList[i]) == False :
                 continue
            lineStr += CfgObjToString(colnames[i]) + ":" + ObjToString(row[i],typeList[i])
            if i < column -1:
                lineStr += ","
            if(colnames[i].lower() == "id"):
                idStr = "\"" + ObjToString(row[i],typeList[i]) + "\""

        if lineStr.endswith(",") :
            lineStr = lineStr[0:len(lineStr)-1]

        file.write("\t" + idStr + ":{" + lineStr + "}")
        if(rowNum < nrows-1):
            file.write(",\n")
    file.write("\n}")
    file.close()

def ObjToString(value,valType):
    if valType == 'int':
        if value == "":
            value = "0"
        return str(int(value))
    elif valType == 'list': 
        return "[" + str(value) + "]"
    elif valType == 'string':
        return "\"" + str(value) + "\""
    else: 
        return str(value) 

def CfgObjToString(value):
    if type(value) == float:
        return str(int(value))
    else: 
        return "\"" + str(value) + "\""

#校验字段格式，符合条件输出到配置中
def CheckFieldFormat(useType):
    if (useType == u"客户端" or useType == u"客户端+服务器"):
        return True
    else:
        return False

def convertToString(value):
    temp = value
    if type(value) == float:
        temp = str(int(value))
    return temp


def converToInt(value):
    temp = value
    if type(value) == float:
        temp = int(value)
    if type(value) == str:
        temp = 0
    return temp


def main(argv):
    #fileName = argv[1]
    #outputDir = argv[2]
    fileName = "F:/BaiduNetdiskDownload/Game/Excel/Excel/contents.xlsx"
    outputDir = "F:/BaiduNetdiskDownload/Game/Excel/Excel/configs/json/"
    open_contents(fileName, outputDir)


if __name__ == "__main__":
    main(sys.argv)
