add_microsoft_stuff()
{
	wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
	sudo dpkg -i packages-microsoft-prod.deb
	rm packages-microsoft-prod.deb
	sudo apt update
}

install_dotnet()
{
	sudo apt install -y dotnet-sdk-6.0
}

if ! apt-cache show dotnet-sdk-6.0 | grep "Package";
then
	add_microsoft_stuff
fi

if ! dpkg -s dotnet-sdk-6.0 | grep "Package";
then
	install_dotnet
fi
