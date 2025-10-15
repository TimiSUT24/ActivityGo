import {useState, useEffect} from 'react';

export default function LocalWeather (){
    const [weather, setWeather] = useState(null);
    const [error, setError] = useState(null);

    useEffect (() => {
        if(navigator.geolocation){
            navigator.geolocation.getCurrentPosition(sucess,failure)
        }
        else{
            setError("GeoLocation is not supported")
        }
    }, []);

    const sucess = async (position) => {
        const {latitude, longitude} = position.coords;

        try{
            const res = await fetch(
                `/api/`
            );
        }
        catch{

        }
    }

    return (
        <div>

        </div>
    )
}