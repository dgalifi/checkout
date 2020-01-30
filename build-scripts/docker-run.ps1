Set-Location -Path ..\

docker stop co
docker rm co

docker run -d -p 5000:5000 --name co co_img

Set-Location -Path .\build-scripts