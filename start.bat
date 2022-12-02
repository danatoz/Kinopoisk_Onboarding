docker build -f DownloaderMoviePremiereService\Dockerfile --force-rm -t kinopoisk_onboarding-downloadermoviesremiereservice .
docker build . --force-rm -t kinopoisk_onboarding-webapi
docker compose up -d
cmd