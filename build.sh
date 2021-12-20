if [ ! -f appsettings.json ];
then
	echo appsettings.json file not found
	echo you can check appsettings.json.sample for an example
	exit
fi

sudo cp appsettings.json "src/iCTF Website"
sudo cp appsettings.json "src/iCTF Discord Bot"
sudo cp appsettings.json "src/iCTF Shared Resources" # useless

cd src
sudo dotnet publish -c release