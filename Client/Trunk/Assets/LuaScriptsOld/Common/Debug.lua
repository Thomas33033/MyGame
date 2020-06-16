-- 输出日志--
function log(str)
    Util.Log(debug.traceback(str));
end

-- 打印字符串--
function print(str)
    Util.Log(debug.traceback(str));
end

-- 错误日志--
function error(str)
    Util.LogError(debug.traceback(str));
end

-- 警告日志--
function warn(str)
    Util.LogWarning(debug.traceback(str));
end