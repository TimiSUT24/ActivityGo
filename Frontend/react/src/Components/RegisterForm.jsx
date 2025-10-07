import {useState} from 'react'
import axios from 'axios'

export default function RegisterForm (){

    const [formData, setFormData] = useState({
        email:"",
        password:"",
    });

    const [message, setMessage] = useState("");

    return (
        <div>
            
        </div>
    )
}