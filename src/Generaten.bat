mkdir Generated

dotnet ChinaPublicCalendarGenerator.dll holiday -r -o:Generated/Holiday.ics
dotnet ChinaPublicCalendarGenerator.dll international-festival -r -o:Generated/InternationalFestival.ics
dotnet ChinaPublicCalendarGenerator.dll solar-terms -r -o:Generated/SolarTerms.ics
dotnet ChinaPublicCalendarGenerator.dll tradition-festival -r -o:Generated/TraditionFestival.ics
dotnet ChinaPublicCalendarGenerator.dll movie-coming -r -o:Generated/MovieComing.ics
dotnet ChinaPublicCalendarGenerator.dll snooker-tournament -r -o:Generated/SnookerTournament.ics