module Client.Types.MovieDb

type Genre = int * string

module Genre =
    // https://api.themoviedb.org/3/genre/movie/list?language=en-US
    let all: Genre list =
        [
            28, "Action"
            12, "Adventure"
            16, "Animation"
            35, "Comedy"
            80, "Crime"
            80, "Crime"
            99, "Documentary"
            18, "Drama"
            10751, "Family"
            14, "Fantasy"
            36, "History"
            27, "Horror"
            10402, "Music"
            9648, "Mystery"
            10749, "Romance"
            878, "Science Fiction"
            10770, "TV Movie"
            53, "Thriller"
            10752, "War"
            37, "Western"
        ]

type Actor = int * string

type Movie =
    {
        Title: string
        Overview: string
    }

type Result =
    {
        Results: Movie list
    }