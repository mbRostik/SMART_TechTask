import React from 'react';
import { useState, useEffect, useRef } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import userManager from '../../AuthFiles/authConfig';
import { NavLink } from 'react-router-dom';
import { ThreeDots } from 'react-loader-spinner';
import { useAuth } from '../AuthProvider';
import config from '../../config.json';
import './NavBar.css'

const NavBar = () => {
    const { user, userData, loading, isAuthorized } = useAuth();

    return (
        <div className="NavBarMain">
            {loading ? (
                <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh' }}>
                    <ThreeDots color="orange" height={80} width={80} />
                </div>
            ) : isAuthorized && userData.fullyRegistered ? (
                    <div className="NavBarMenuUnAuth">

                        <NavLink to="/" className="nav-item">
                            <div>Home</div>
                        </NavLink>


                        <NavLink to="/employees" className="nav-item">
                        <div>Employees</div>
                    </NavLink>

                    <NavLink to="/about-us" className="nav-item">
                        <div>Approval Requests</div>
                    </NavLink>


                    <NavLink to="/projects" className="nav-item">
                        <div>Projects</div>
                    </NavLink>

                    <NavLink to="/profile">
                        <img
                            className="nav-profile"
                            src={userData.photo ? `data:image/jpeg;base64,${userData.photo}` : "profile.png"}
                            alt={userData.nickName || "Profile"}
                        />
                    </NavLink>
                </div>
            ) : null}
        </div>
    );
};



export default NavBar;




