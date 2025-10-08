import {useState} from 'react';
import '../CSS/HomePage.css';

export default function HomePage(){

    return(
        <body className="home-body">
            <div className="mario-island">   
                <img src="./IMG/marioisland.png" alt="" className="mario-island"/>        
            </div>
            <div className ="activity-category">
                <img src="/IMG/Greencircle.png" alt="" className="category-img" width={100}/>
                <img src="/IMG/circle.png" alt="" className="category-img" width={100}/>
                <img src="/IMG/circle.png" alt="" className="category-img" width={100}/>
            </div>

            <div className="activity-list">
                <div className="activity-card" id="card1">

                </div>

                <div className="activity-card"id="card2">

                </div>

                <div className="activity-card" id="card3">

                </div>

                <div className="activity-card" id="card4">

                </div>


            </div>
        </body>      
    )
}