# ImaginaryCTF

ImaginaryCTF is a platform that brings the beauty of CTF Competitions to discord, releasing a challenge every day.

**Live Website:** [https://imaginaryctf.org](https://imaginaryctf.org)

**Live Discord Bot:** [https://discord.gg/vReKWNxPuE](https://discord.gg/vReKWNxPuE)

## Frequently Asked Questions

**This project is composed by 2 different apps. Do I need to run both of them in order for it to work?**

ImaginaryCTF is composed by a website and a discord bot. The website is the responsible for challenge submissions while the discord bot is responsible for releasing them.
If you decide not to use the website then you would need to manually insert the challenges in the database. If you decide not to use the bot then you would need to manually release the challenges by modifying them in the database.

**What are the default admin credentials?**

You can login with the username `admin` and the password `Admin123!`

## Requirements

If you are planning to use the full version of this app you will need access to:

- A MySQL server
- A SMTP server (either gmail, outlook or a custom one, it's up to you)

## Customization

General configuration is located at : [appsettings.json](appsettings.json)

If you do not know how to change the platform to fit your needs or do not want to do that, please consider supporting me, as supporters can ask me to do that and more advanced changes to the platform. Those changes include but are not restricted to:

- Support for SQLite instead of MySQL 
- Discarding the Discord Bot and make it a only-Website platform
- Discarding the Website and make it a only-Discord platform
- Much more, feel free to contact me to discuss your needs

## Installation

#### Clone ImaginaryCTF

```
git clone https://github.com/Et3rnos/ImaginaryCTF
```

#### Install .NET Core SDK 6.0

Using Ubuntu 20.04? Just run `bash install-ubuntu-20.04.sh`. Otherwise:

Please visit [https://docs.microsoft.com/en-us/dotnet/core/install/linux-ubuntu](https://docs.microsoft.com/en-us/dotnet/core/install/linux-ubuntu) for instructions on how to install .NET Core SDK 6.0 in your Ubuntu distribution.

#### Customize it

Please view [Customization Section](#Customization)

#### Publish it

```
dotnet publish -c release
```

NOTE: published apps are usually under `[project_folder]/bin/Release/net6.0/publish/` directories.

#### Execute both apps

```
dotnet "path_to_the_published_discord_bot.dll" &
dotnet "path_to_the_published_website.dll" &
```

## Disclaimer

ImaginaryCTF comes with absolutely no warranties.

## Contact Me

The easier way to contact me is through Discord. My username is Et3rnos#6556.

## Support Me

You can support me either via Patreon: [https://www.patreon.com/et3rnos](https://www.patreon.com/et3rnos)

Or via Paypal: [https://www.paypal.com/donate?hosted_button_id=JSM77LBGLJ3FG](https://www.paypal.com/donate?hosted_button_id=JSM77LBGLJ3FG)
