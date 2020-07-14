SysTimer = class("SysTimer")

function SysTimer:ctor()
    self.mTimerList = {}
end

--repeatCount -1:循环调用 大于1:执行指定次数
function SysTimer:AddTimer(duration, repeatCount,func, target)
    if repeatCount < 0 then
        repeatCount = -1
    end

    local timer = Timer.New(function(_timer)
        func(target)
        if _timer.running == false then
            self:DeleteTimer(_timer)
        end
    end,duration, repeatCount, false)

    LuaTools.TableInsert(self.mTimerList,timer)
    timer:Start()
    return timer;
end

function SysTimer:AutoClose( ... )
    -- body
end

function SysTimer:DeleteTimer(timer)
    --销毁时间
    print("SysTimer:DeleteTimer()");
    if timer then
        timer:Stop()
        print("timer:Stop()")
        LuaTools.TableDelete(self.mTimerList,timer)
    end
end

function SysTimer:OnDestroy()
    for i,v in pairs(self.mTimerList) do
        if self.mTimerList[i] then
            self.mTimerList[i]:Stop()
        end
        self.mTimerList[i] = nil
    end
    self.mTimerList = {}
end


