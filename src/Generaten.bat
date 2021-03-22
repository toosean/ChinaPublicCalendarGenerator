mkdir Generated

dotnet ChinaPublicCalendarGenerator.dll holiday -r -o:Generated/Holiday.ics
dotnet ChinaPublicCalendarGenerator.dll international-festival -r -o:Generated/InternationalFestival.ics
dotnet ChinaPublicCalendarGenerator.dll solar-terms -r -o:Generated/SolarTerms.ics
dotnet ChinaPublicCalendarGenerator.dll tradition-festival -r -o:Generated/TraditionFestival.ics
dotnet ChinaPublicCalendarGenerator.dll movie-coming -r -o:Generated/MovieComing.ics
dotnet ChinaPublicCalendarGenerator.dll game-launch -r -o:Generated/GameLaunch.ics
dotnet ChinaPublicCalendarGenerator.dll snooker-tournament -r -o:Generated/SnookerTournament.ics
dotnet ChinaPublicCalendarGenerator.dll football-tournament -r -o:Generated/FootballTournament.ics
dotnet ChinaPublicCalendarGenerator.dll nba-tournament -r -o:Generated/NBATournament.ics
dotnet ChinaPublicCalendarGenerator.dll epic -r -o:Generated/EPIC.ics
