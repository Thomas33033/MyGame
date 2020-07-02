# -*- coding: utf-8 -*-
import sys
import xlrd
import pdb
import types

reload(sys)
sys.setdefaultencoding('utf-8')

inputDir = ""


def open_excel(file):
    try:
        data = xlrd.open_workbook(filename=file, encoding_override="utf-8")
        return data
    except Exception, e:
        print str(e)


# 解析目录文件
# @param file 目录文件名
def open_contents(file, outputDir, isClient):
    data = open_excel(file)
    table = data.sheet_by_name("first")
    nrows = table.nrows
    for index in range(1, nrows):
        row = table.row_values(index)
        input = inputDir + row[0]
        sheet = row[1]
        out = row[2]
        luaName = row[3]
        type = row[4]
        startCol = converToInt(row[5])

        tableData = open_excel(input)

        if sheet == "":
            curTable = tableData.sheet_by_index(0)
        else:
            curTable = tableData.sheet_by_name(sheet)

        # 计算哪些字段需要打进去
        allNeedFiledArr = []
        colnames = curTable.row_values(1)
        useTypeList = curTable.row_values(3)
        for colIndex in range(0, len(colnames)):
            fieldName = colnames[colIndex]
            useType = useTypeList[colIndex]
            if (fieldName != ""):
                if isClient:
                    if (useType == u"客户端" or useType == u"客户端+服务器"):
                        allNeedFiledArr.append(fieldName)
                else:
                    if (useType == u"服务器" or useType == u"客户端+服务器"):
                        allNeedFiledArr.append(fieldName)

        # 没有需要的字段就不打表
        if (len(allNeedFiledArr) <= 0):
            continue

        strType = ""
        if(type == 2):
            strType = u"嵌套类型"
        elif(type == 1):
            strType = u"基本类型"
        elif(type == 3):
            strType = u"数组类型"

        # 表格类型 1：基本类型， 2：嵌套类型  3:数组类型
        print("*** Converting: " + strType + "  " + input + "  ")
        if(type == 2):
            convert_complex_table(out, luaName, curTable,
                                  allNeedFiledArr, outputDir, startCol)
        elif(type == 1):
            convert_simple_table(out, luaName, curTable,
                                 allNeedFiledArr, outputDir, startCol)
        elif(type == 3):
            convert_array_table(out, luaName, curTable,
                                allNeedFiledArr, outputDir, startCol)


def convert_array_table(fileName, luaName, table, allNeedFiledArr, outputDir, keyIndex=0, index=4):
    nrows = table.nrows  # 行数
    colnames = table.row_values(1)
    file = open(outputDir + fileName+".lua", "wb")
    file.write(luaName + " = " + luaName + " or {}\r\n")
    typeList = table.row_values(2)
    for rownum in range(index, nrows):
        row = table.row_values(rownum)
        if row and row[keyIndex] != "":
            file.write(luaName+"[")
            file.write(convertToString(row[keyIndex]))
            file.write("]")
            file.write("=")
            parse_line(file, colnames, row, allNeedFiledArr, typeList)
            file.write("}")
            file.write("\r\n")
    file.close()

# @param fileName:输出文件名称
# @param file:输入文件名
# @param textList:字段是text
# @param textList:字段是text
# @param floatList:字段是float
# @param colNameIndex:属性字段行
# @param index:真实数据开始行，跳过注释行
# @param colnames:属性字段表
# @param keyIndex:那个字段为空的话，则判定该行是空行


def convert_simple_table(fileName, luaName, table, allNeedFiledArr, outputDir, keyIndex=0, index=4):
    nrows = table.nrows  # 行数
    colnames = table.row_values(1)
    file = open(outputDir + fileName+".lua", "wb")
    file.write(luaName + " = " + luaName + " or {}\r\n")
    typeList = table.row_values(2)
    for rownum in range(index, nrows):
        row = table.row_values(rownum)
        if row and row[keyIndex] != "":
            file.write(luaName+"[\"")
            file.write(convertToString(row[keyIndex]))
            file.write("\"]")
            file.write("=")
            parse_line(file, colnames, row, allNeedFiledArr, typeList)
            file.write("}")
            file.write("\r\n")
    file.close()


# @param fileName:输出文件名称
# @param file:输入文件名
# @param colNameIndex:属性字段行
# @param index:真实数据开始行，跳过注释行
# @param colnames:属性字段表
# @param keyIndex:那个字段为空的话，则判定该行是空行
def convert_complex_table(fileName, luaName, table, allNeedFiledArr, outputDir, keyIndex=0, index=4):
    nrows = table.nrows  # 行数
    colnames = table.row_values(1)
    list = []
    file = open(outputDir + fileName+".lua", "wb")
    file.write(luaName + " = " + luaName + " or {}\r\n")
    hasLast = False
    typeList = table.row_values(2)
    for rownum in range(index, nrows):
        row = table.row_values(rownum)
        isHead = row and row[keyIndex] != ""
        if isHead:
            if hasLast:
                file.write("}")
            file.write("\r\n"+luaName+"[\"")
            file.write(convertToString(row[keyIndex]))
            file.write("\"]")
            file.write("=")
            file.write("{")
        elif hasLast:
            file.write(",")
        file.write("\r\n")
        parse_line(file, colnames, row, allNeedFiledArr, typeList)
        file.write("}")
        hasLast = True
    file.write("}")
    file.close()

# 解析一对键值
def parse_line(file, col, row, allNeedFiledArr, typeList):
    for i in range(len(col)):
        key = col[i]
        value = row[i]
        # if i ==0:
        # continue
        if i == 0:
            file.write("{")
        if (not value) and value != 0:
            continue
        if key in allNeedFiledArr:
            dataType = "string"
            if len(typeList[i]) > 0:
                dataType = typeList[i]
            file.write(convertToString(key))
            file.write(" = ")
            if type(value) == float:
                if value % 1 == 0:
                    value = str(int(value))
                else:
                    value = str(value)

                if dataType == "string":
                    if len(value) == 0:
                        file.write("\"\"")
                    else:
                        file.write("\"")
                        file.write(value)
                        file.write("\"")
                elif dataType == "array":
                    if len(value) == 0:
                        file.write("{}")
                    else:
                        file.write("{")
                        file.write(value)
                        file.write("}")
                else:
                    file.write(value)
            else:
                value = str(value)
                if len(value) == 0:
                    file.write("\"\"")
                else:
                    if dataType == "string":
                        file.write("\"")
                        file.write(value)
                        file.write("\"")
                    elif dataType == "array" or dataType == ('int[]') or dataType == 'string[]':
                        file.write("{")
                        file.write(value)
                        file.write("}")
                    elif dataType.find('Dictionary') == 0:
                        dictStr = '{'
                        if len(value) >= 2 :
                            ##忽略掉开始结尾的{}
                            value = value[1:len(value)-1]

                            kvArr = value.split(',')
                            count = len(kvArr)
                            for j in range(0, count):
                                arr = kvArr[j].split(':')
                                dictStr = dictStr + '[{0}]={1}'.format(arr[0], arr[1])
                                
                                if count - 1 != j :
                                    dictStr = dictStr + ','
                        dictStr = dictStr + '}'
                        file.write(dictStr)
                    else:
                        file.write(value)

            if i < len(col)-1:
                file.write(",")


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
    if len(argv) >= 3 and argv[3] != "0":
        isClient = True
    else:
        isClient = False
    open_contents(fileName, outputDir, isClient)


if __name__ == "__main__":
    main(sys.argv)
