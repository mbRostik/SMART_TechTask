import { useEffect, useState } from 'react';
import './App.css';
import userManager from './AuthFiles/authConfig';

function App() {
   
    const onLogin = () => {
        userManager.signinRedirect();
    };

    const onLogout = async () => {
        await userManager.signoutRedirect();
        navigate('/');
    };


    return (
        <div>
            <h1 id="tabelLabel">Weather forecast</h1>
            <p>This component demonstrates fetching data from the server.</p>
            <div className="nav-item">

                <button onClick={onLogin} className="NavBarButton_Login">Login / Sign Up</button>
                <button className="signout-button" onClick={onLogout}>
                    SIGN OUT
                </button>
            </div>
        </div>

    );
}

export default App;