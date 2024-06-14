import React from 'react';
import { useState, useEffect, useRef } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import userManager from '../../AuthFiles/authConfig';
import { NavLink } from 'react-router-dom';
import { ThreeDots } from 'react-loader-spinner';
import { useAuth } from './../AuthProvider';
import config from '../../config.json';
import './Profile.css'
import { ToastContainer, toast } from 'react-toastify';

const Profile = () => {
    const [isHovered, setIsHovered] = useState(false);
    const { user, userData, loading, isAuthorized, setLoadingState,
        setIsAuthorizedState,
        setUserState,
        setUserDataState } = useAuth();




    const handleMouseEnter = () => {
        setIsHovered(true);
    }

    const handleMouseLeave = () => {
        setIsHovered(false);
    }

    const handleImageUpload = (e) => {
        setLoadingState(true);
        const file = e.target.files[0];
        const maxSize = 5 * 1024 * 1024;
        const maxResolution = 1920;

        if (file && !isImageFile(file)) {
            setLoadingState(false);
            return;
        }

        if (file && file.size > maxSize) {

            setLoadingState(false);
            return;
        }

        if (file) {
            const img = new Image();
            img.onload = () => {
                const width = img.width;
                const height = img.height;
                if (width > maxResolution || height > maxResolution) {
                    console.log("Too huge photo");
                    setLoadingState(false);
                    return;
                }
                else {
                    const reader = new FileReader();
                    reader.onloadend = async () => {
                        const imageData = reader.result;
                        const blob = new Blob([new Uint8Array(imageData)], { type: file.type });
                        const base64Avatar = await new Promise((resolve) => {
                            const reader = new FileReader();
                            reader.onloadend = () => resolve(reader.result.split(',')[1]);
                            reader.readAsDataURL(blob);

                        });
                        try {
                            const accessToken = await userManager.getUser().then(user => user.access_token);
                            const response = await fetch(`${config.apiBaseUrl}/UploadProfilePhoto`, {
                                method: 'POST',
                                headers: {
                                    'Authorization': `Bearer ${accessToken}`,
                                    'Content-Type': 'application/json'
                                },
                                body: JSON.stringify({ avatar: base64Avatar })
                            });
                            const data = await response.json();
                            setUserDataState(data);


                        } catch (err) {

                            console.error('Error while sending the request', err);
                        } finally {
                            setLoadingState(false);
                        }
                    };
                    reader.readAsArrayBuffer(file);
                }
            };
            img.src = URL.createObjectURL(file);
        }
    };

    const isImageFile = (file) => {
        const acceptedImageTypes = ['image/jpeg', 'image/png', 'image/svg+xml', 'image/webp'];

        return acceptedImageTypes.includes(file.type);
    };


    const handleImageDelete = async (e) => {
        setLoadingState(true);
        if (userData.photo == null || (Array.isArray(userData.photo) && userData.photo.length === 0) || userData.photo == '') {
           
            e.target.value = null;

            setLoadingState(false);
            return;
        }

        try {

            const accessToken = await userManager.getUser().then(user => user.access_token);
            await fetch(`${config.apiBaseUrl}/UploadProfilePhoto`, {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${accessToken}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ avatar: "" })
            });
            userData.photo = null;
           
        }
        catch (err) {
            toast.error(err, {
                position: "top-right",
                autoClose: 5000,
                hideProgressBar: false,
                closeOnClick: true,
                pauseOnHover: true,
                draggable: true,
                progress: undefined,
            });
        }
        setLoadingState(false);
    };


    const [peoplePartnerID, setPeoplePartnerID] = useState(null);

    const [isEditing, setIsEditing] = useState(false);

    useEffect(() => {
        if (userData) {
            setPeoplePartnerID(userData.peoplePartnerID || 'None');
        }
    }, [userData]);


    const handleSaveChanges = async () => {
        try {
            setLoadingState(true);
            const accessToken = await userManager.getUser().then(user => user.access_token);
            const response = await fetch(`${config.apiBaseUrl}/UpdateProfile`, {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${accessToken}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ partner: peoplePartnerID })

            });
            if (!response.ok) throw new Error(`HTTP error! Status: ${response.status}`);

            const accessToken2 = await userManager.getUser().then(user => user.access_token);
            const response2 = await fetch(`${config.apiBaseUrl}/GetUserProfile`, {
                headers: { 'Authorization': `Bearer ${accessToken2}` }
            });
            setUserDataState(await response2.json());
            setIsEditing(false);
        } catch (error) {
            console.log('Error while sending the request to the UserService', error);
        }
        finally {
            setLoadingState(false);
        }
    };



    return (
        <div className="ProfileMain">
            {loading ? (
                <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh' }}>
                    <ThreeDots color="orange" height={80} width={80} />
                </div>
            ) : isAuthorized === false ? (
                <div>UnAuthorized</div>
            ) : (

                <div className="ProfileDiv">
                    <div className="Left_ProfileDiv">
                        <div className="avatar-container" onMouseEnter={handleMouseEnter} onMouseLeave={handleMouseLeave}>
                            <img src={userData.photo ? `data:image/jpeg;base64,${userData.photo}` : "/NoPhoto.jpg"} alt="Avatar" className="avatar" />
                            <div className="buttons-container">
                                <label className="edit-button">
                                    New
                                    <input type="file" name="clientAvatar" accept="image/*" onChange={handleImageUpload} style={{ display: 'none' }} capture="false" />
                                </label>

                                <label className="edit-button" onClick={handleImageDelete}>
                                    Delete
                                </label>
                            </div>
                        </div>
                    </div>
                    <div className="Right_ProfileDiv">
                        
                                <h3>{userData.fullName}</h3>
                                <div className="Right_ProfileDiv_Title">
                                    <input
                                        type="text"
                                        value={peoplePartnerID }
                                        onChange={(e) => setPeoplePartnerID(e.target.value)}
                                        className="input-date"
                                    />
                                     -(Input HR Name to change)
                                </div>
                                <p>{userData.position}</p>
                                <p>{userData.status}</p>
                                <p>{userData.subdivision}</p>
                                <div onClick={handleSaveChanges}  className="Brown_Profile_Button">Save Changes</div>
                    </div>


                </div>


            )}
        </div>
    );
};



export default Profile;




