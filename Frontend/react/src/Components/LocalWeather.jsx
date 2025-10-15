import {useState, useEffect} from 'react';
import api from "../lib/api";

export default function LocalWeather (){
    const [weather, setWeather] = useState(null);
    const [error, setError] = useState(null);

    useEffect (() => {
        if(navigator.geolocation){
            navigator.geolocation.getCurrentPosition(success,failure)
        }
        else{
            setError("GeoLocation is not supported")
        }
    }, []);

    const success = async (position) => {
        const {latitude, longitude} = position.coords;

        try{
            const res = await api.get(
                `/api/Weather/localweather?lat=${latitude}&lon=${longitude}`
            );
            setWeather(res.data);

        }
        catch(error){
            console.error("Failed to fetch weather", error);
            setError(error.response?.data?.detail);
        }
    };

    const failure = (error) => {
        setError("Could not get location: " + error.message);
    };

    if(error){
        return <p>Error: {error}</p>
    }

    if(!weather){
        return <p>Loading weather...</p>
    }

    const current = weather.slices[0]; //current hour

    return (
        <div className="weather-container">
            <h2 className="weather-title">
                Local Weather
            </h2>
            <div className="weather-info">
                <p>
                {Math.round(current.temperatureC)}°C - {current.conditionText}
            </p>
                <img src={`https://openweathermap.org/img/wn/${current.conditionIconUrl}@2x.png`} alt={current.conditionText} width={60} height={60} />
            </div>                     
        </div>
    )
}