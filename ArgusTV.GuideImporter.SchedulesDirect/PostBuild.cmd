IF %3 == Release goto end
  MD %1..\..\..\ArgusTV.GuideImporter\bin\%3\Plugins
  MD %1..\..\..\ArgusTV.GuideImporter\bin\%3\Plugins\SchedulesDirect
  
  copy %1ArgusTV.DataContracts.dll %1..\..\..\ArgusTV.GuideImporter\bin\%3\Plugins\SchedulesDirect
  copy %1ArgusTV.GuideImporter.Interfaces.dll %1..\..\..\ArgusTV.GuideImporter\bin\%3\Plugins\SchedulesDirect
  copy %1ArgusTV.GuideImporter.Interfaces.pdb %1..\..\..\ArgusTV.GuideImporter\bin\%3\Plugins\SchedulesDirect

  copy %1ArgusTV.GuideImporter.SchedulesDirect.dll %1..\..\..\ArgusTV.GuideImporter\bin\%3\Plugins\SchedulesDirect
  copy %1ArgusTV.GuideImporter.SchedulesDirect.pdb %1..\..\..\ArgusTV.GuideImporter\bin\%3\Plugins\SchedulesDirect
  copy %1AvailableChannels.config %1..\..\..\ArgusTV.GuideImporter\bin\%3\Plugins\SchedulesDirect
  copy %1..\..\ArgusTV.GuideImporter.SchedulesDirect.dll.config %1..\..\..\ArgusTV.GuideImporter\bin\%3\Plugins\SchedulesDirect
:end
  copy %1..\..\ArgusTV.GuideImporter.SchedulesDirect.dll.config %1ArgusTV.GuideImporter.SchedulesDirect.dll.config
exit 0
