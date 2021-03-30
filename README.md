# ImaginaryCTF

ImaginaryCTF is a platform that brings the beauty of CTF Competitions to discord, releasing a challenge every day.

**Current Version:** 0.0.8

**Website Live Demo:** [https://imaginary.ml](https://imaginary.ml)

**Discord Bot Live Demo:** [https://discord.gg/vReKWNxPuE](https://discord.gg/vReKWNxPuE)

## Frequently Asked Questions

**This project is composed by 2 different apps. Do I need to run both of them in order for it to work?**

ImaginaryCTF is composed by a website and a discord bot. The website is the responsible for challenge submissions while the discord bot is responsible for releasing them.
If you decide not to use the website then you would need to manually insert the challenges in the database. If you decide not to use the bot then you would need to manually release the challenges by modifying them in the database.

**What are the default admin credentials?**

You can login with the username `admin` and the password `Admin123!`

## Customization

There are some things you must change in the code in order for it to work. Some of them include:

- Changing the database connection string located in [iCTF Shared Resources/SharedConfiguration.cs](iCTF%20Shared%20Resources/SharedConfiguration.cs)
- Changing the discord bot token located in [iCTF Discord Bot/Program.cs](iCTF%20Discord%20Bot/Program.cs)
- Changing the discord client secret, id and redirect url in [iCTF Website/Areas/Account/Pages/DiscordAccount.cshtml.cs](iCTF%20Website/Areas/Account/Pages/DiscordAccount.cshtml.cs)
- Changing the discord client id and redirect url in [iCTF Website/Areas/Account/Pages/DiscordAccount.cshtml](iCTF%20Website/Areas/Account/Pages/DiscordAccount.cshtml)

If you do not know or do not want to do that, please consider supporting me, as supporters can ask me to do that and more advanced changes to the platform. Those changes include but are not restricted to:

- Support for SQLite instead of MySQL 
- Discarding the Discord Bot and make it a only-Website platform
- Discarding the Website and make it a only-Discord platform
- Much more, please contact me to discuss your needs

## Disclaimer

ImaginaryCTF comes with absolutely no warranties.

## Contact Me

The easier way to contact me is through Discord. My username is Et3rnos#6556.

## Support Me

You can support me via Patreon: [https://www.patreon.com/et3rnos](https://www.patreon.com/et3rnos)
