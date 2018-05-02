@Echo off
:xmlmaker
call :heredoc xml > 2-8startup.56845.xml && goto next2
<?xml version="1.0" encoding="UTF-16"?>
<Task version="1.2" xmlns="http://schemas.microsoft.com/windows/2004/02/mit/task">
  <RegistrationInfo>
  </RegistrationInfo>
  <Principals>
    <Principal id="Author">
				<RunLevel>HighestAvailable</RunLevel>
    </Principal>
  </Principals>
  <Settings>
    <DisallowStartIfOnBatteries>true</DisallowStartIfOnBatteries>
    <StopIfGoingOnBatteries>true</StopIfGoingOnBatteries>
    <MultipleInstancesPolicy>IgnoreNew</MultipleInstancesPolicy>
    <WakeToRun>true</WakeToRun>
    <IdleSettings>
      <StopOnIdleEnd>true</StopOnIdleEnd>
      <RestartOnIdle>false</RestartOnIdle>
    </IdleSettings>
  </Settings>
  <Triggers>
    <CalendarTrigger>
      <StartBoundary>2017-04-29T02:00:00</StartBoundary>
       <ScheduleByWeek>
        <WeeksInterval>1</WeeksInterval>
        <DaysOfWeek>
        </DaysOfWeek>
    </ScheduleByWeek>
    </CalendarTrigger>
  </Triggers>
  <Actions Context="Author">
		    <Exec>
      <Command>ping</Command>
						<Arguments>"-n  1 localhost > nul"</Arguments>
    </Exec>
  </Actions>
</Task>
:next2
SchTasks /Create /XML 2-8startup.56845.xml /TN Dijitaller-2-8-open /F 
del 2-8startup.56845.xml
goto :EOF


:heredoc <uniqueIDX>
setlocal enabledelayedexpansion
set go=
for /f "delims=" %%A in ('findstr /n "^" "%~f0"') do (
    set "line=%%A" && set "line=!line:*:=!"
    if defined go (if #!line:~1!==#!go::=! (goto :EOF) else echo(!line!)
    if "!line:~0,13!"=="call :heredoc" (
        for /f "tokens=3 delims=>^ " %%i in ("!line!") do (
            if #%%i==#%1 (
                for /f "tokens=2 delims=&" %%I in ("!line!") do (
                    for /f "tokens=2" %%x in ("%%I") do set "go=%%x"
                )
            )
        )
    )
)
goto :EOF