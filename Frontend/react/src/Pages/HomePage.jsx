import {useState} from 'react';
import '../CSS/HomePage.css';

export default function HomePage(){

    return(
        <div className="home-content">
            <div className="mario-island">   
                <img src="/IMG/marioisland.png" alt="" className="mario-island"/>        
            </div>
            <div className ="activity-category">
                <img src="/IMG/Greencircle.png" alt="" className="category-img" width={100}/>
                <img src="/IMG/mushroomcircle.png" alt="" className="category-img" width={100}/>
                <img src="/IMG/bluecircle.png" alt="" className="category-img" width={100}/>
            </div>

            <div className="activity-list">
                <div className="activity-card" id="card1">
                    <div>
                        <img src="/IMG/mariotennis.png" alt="" width={170} height={140} className="activity-img" />
                    </div>
                    
                    <button className="activity-card-btn" id="activity-btn1">Boka här</button>
                </div>

                <div className="activity-card"id="card2">
                    <div>
                        <img src="/IMG/toadTraining.png" alt=""  width={170} height={140} className="activity-img"/>
                    </div>
                      <button className="activity-card-btn" id="activity-btn2">Boka här</button>
                </div>
                

                <div className="activity-card" id="card3">
                    <div>
                        <img src="/IMG/Marioyoga.png" alt=""  width={170} height={140} className="activity-img"/>
                    </div>
                      <button className="activity-card-btn" id="activity-btn3">Boka här</button>
                </div>

                <div className="activity-card" id="card4">
                       <div>
                        <img src="/IMG/luigiclimbing.png" alt=""  width={170} height={140} className="activity-img"/>
                    </div>
                      <button className="activity-card-btn" id="activity-btn4">Boka här</button>
                </div>
            </div>
        </div>      
    )
}