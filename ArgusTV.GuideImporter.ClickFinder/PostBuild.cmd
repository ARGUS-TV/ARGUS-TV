IF %3 == Release goto end
    MD %1..\..\..\ArgusTV.GuideImporter\bin\%3\Plugins
    MD %1..\..\..\ArgusTV.GuideImporter\bin\%3\Plugins\ClickFinder
  
  copy %1ArgusTV.DataContracts.dll %1..\..\..\ArgusTV.GuideImporter\bin\%3\Plugins\ClickFinder
  copy %1ArgusTV.GuideImporter.Interfaces.dll %1..\..\..\ArgusTV.GuideImporter\bin\%3\Plugins\ClickFinder
  copy %1ArgusTV.GuideImporter.Interfaces.pdb %1..\..\..\ArgusTV.GuideImporter\bin\%3\Plugins\ClickFinder
  
  copy %1ArgusTV.GuideImporter.ClickFinder.dll %1..\..\..\ArgusTV.GuideImporter\bin\%3\Plugins\ClickFinder
  copy %1ArgusTV.GuideImporter.ClickFinder.pdb %1..\..\..\ArgusTV.GuideImporter\bin\%3\Plugins\ClickFinder
  rem copy %1..\..\ArgusTV.GuideImporter.ClickFinder.dll.config %1..\..\..\ArgusTV.GuideImporter\bin\%3\Plugins\ClickFinder
:end
  rem copy %1..\..\ArgusTV.GuideImporter.ClickFinder.dll.config %1ArgusTV.GuideImporter.ClickFinder.dll.config
exit 0
