Note that in order to correctly interoperate with the database access from entity framework into WSL2, all table names were corrected to Microsoft UpperCamelCase to satisfy how EntityFramework queries the database. E.g. inspectionitemcannedresponses -> InspectionItemCannedResponses. No other schema changes are different between development and production. 

