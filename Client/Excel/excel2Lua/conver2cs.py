# -*- coding: utf-8 -*-
import sys
import xlrd
import pdb
import types

#reload(sys)
#sys.setdefaultencoding('utf-8')

inputDir = ""


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

    luaNameList = []

    for index in range(1, nrows):
        row = table.row_values(index)
        input = inputDir + row[0]
        sheet = row[1]
        out = row[2]
        luaName = row[3]
        #type1 = row[4]
        #startCol = converToInt(row[5])
        outputCS = converToInt(row[6])

        if outputCS == 0:
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

        print("Generate " + out + ".cs from " + input)
        generate_cs_code(out, luaName, curTable, outputDir)
        luaNameList.append(luaName)

    generate_tableManager(luaNameList, outputDir)


def generate_cs_code(fileName, luaName, table, outputDir):
    file = open(outputDir + fileName+".cs", "w")
    file.write("using System;\r\n")
    file.write("using System.Collections.Generic;\r\n\r\n")
    file.write("namespace DataManager\r\n")
    file.write("{\r\n")
    file.write("    public class " + luaName + "\r\n")
    file.write("    {\r\n")
    colnames = table.row_values(1)
    typeList = table.row_values(2)
    useTypeList = table.row_values(3)
    for colIndex in range(0, len(colnames)):
        fieldName = colnames[colIndex]
        typeName = typeList[colIndex]
        useType = useTypeList[colIndex]

        if typeName == "uint32":
            typeName = "uint"
        elif typeName == "array":
            typeName = "object[]"
        elif typeName.find("Dictionary") >= 0:
            typeName = "Dictionary<string,object>"

        if (fieldName != ""):
            if (useType == u"客户端" or useType == u"客户端+服务器"):
                file.write("        public " + typeName +
                           " " + fieldName + ";\r\n\r\n")
    file.write("    }\r\n")
    file.write("}")
    file.close()


def generate_tableManager(luaNameList, outputDir):
    file = open(outputDir + "TableManager.cs", "wb")
    file.write("using System;\r\n\r\n")
    file.write("namespace DataManager\r\n")
    file.write("{\r\n")
    file.write("\
    public class TableManager\r\n\
    {\r\n\
        static TableManager _instance = null;\r\n\
        static public TableManager GetInstance()\r\n\
        {\r\n\
            if (_instance == null)\r\n\
            {\r\n\
                _instance=new TableManager();\r\n\
            }\r\n\
            return _instance;\r\n\
        }\r\n")

    for luaName in luaNameList:
        file.write("        private TableData<" + luaName +
                   "> " + luaName + "_instance = null;\r\n")

    file.write("        private TableManager()\r\n")
    file.write("        {\r\n")
    for luaName in luaNameList:
        file.write("            " + luaName + "_instance = TableData<" +
                   luaName + ">.GetInstance();\r\n")
    file.write("        }\r\n")

    file.write("        public void LoadTableData(string dataPath)\r\n")
    file.write("        {\r\n")
    for luaName in luaNameList:
        file.write("            " + luaName + "_instance.LoadData(dataPath, \"" +
                   luaName + "\");\r\n")
    file.write("        }\r\n")

    for luaName in luaNameList:
        file.write("        public " + luaName +
                   "[] get_" + luaName + "_list()\r\n")
        file.write("        {\r\n")
        file.write("            return " + luaName +
                   "_instance.GetDataList();\r\n")
        file.write("        }\r\n")

    for luaName in luaNameList:
        file.write("        public " + luaName + " get_" +
                   luaName + "_byKey(UInt32 id)\r\n")
        file.write("        {\r\n")
        file.write("            return " + luaName +
                   "_instance.GetDataByKey(id);\r\n")
        file.write("        }\r\n")

    file.write("    }\r\n")
    file.write("}")
    file.close()


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
    fileName = argv[1]
    outputDir = argv[2]
    open_contents(fileName, outputDir)


if __name__ == "__main__":
    main(sys.argv)
