CREATE TABLE [dbo].Szenzorok
(
	szenzorAzon  varchar(5) not null primary key,
    szenzorTípus varchar(35) NOT NULL,
    szenzorErtek int NOT NULL,
    szenzorErtekTartomany varchar(35) NOT NULL,
    mertekEgyseg varchar(35) NOT NULL,
    szenzorHely varchar(35) NOT NULL
	
)
