import React, { useState } from 'react';
import axios from 'axios';
import { useAuth } from '../AuthProvider';
import config from '../../config.json'; 
import userManager from '../../AuthFiles/authConfig';

const FinishUserRegistrationForm = () => {
    const [employeeStatus, setEmployeeStatus] = useState('Active');
    const [position, setPosition] = useState('Employee');
    const [subdivision, setSubdivision] = useState('HR');
    const [partnerName, setPartnerName] = useState('');
    const [dayOffCount, setDayOffCount] = useState(0);
    const { user, userData, loading, isAuthorized, setLoadingState,
        setIsAuthorizedState,
        setUserState,
        setUserDataState, chats, activeChatId,
        setActiveChatId, unknownsmbData, setunknownsmbDataState, hubConnection } = useAuth();
    const handleSubmit = async (e) => {
        e.preventDefault();
        

        const data = {
            EmployeeStatus: employeeStatus,
            Position: position,
            Subdivision: subdivision,
            PartnerName: partnerName,
            DayOffCount: dayOffCount,
        };

        try {
            const accessToken = await userManager.getUser().then(user => user.access_token);

            const response = await fetch(`${config.apiBaseUrl}/FinishRegistration`, {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${accessToken}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(data)
            });

            console.log('Response:', response.data);
        } catch (error) {
            console.error('Error:', error);
        }
    };

    return (
        <form onSubmit={handleSubmit}>
            <div>
                <label>Employee Status:</label>
                <select value={employeeStatus} onChange={(e) => setEmployeeStatus(e.target.value)}>
                    <option value="Active">Active</option>
                    <option value="Inactive">Inactive</option>
                </select>
            </div>

            <div>
                <label>Position:</label>
                <select value={position} onChange={(e) => setPosition(e.target.value)}>
                    <option value="HRManager">HR Manager</option>
                    <option value="Employee">Employee</option>
                    <option value="ProjectManager">Project Manager</option>
                    <option value="Administrator">Administrator</option>
                </select>
            </div>

            <div>
                <label>Subdivision:</label>
                <select value={subdivision} onChange={(e) => setSubdivision(e.target.value)}>
                    <option value="HR">HR</option>
                    <option value="IT">IT</option>
                </select>
            </div>

            <div>
                <label>Partner Name:</label>
                <input
                    type="text"
                    value={partnerName}
                    onChange={(e) => setPartnerName(e.target.value)}
                />
            </div>

            <div>
                <label>Day Off Count:</label>
                <input
                    type="number"
                    value={dayOffCount}
                    onChange={(e) => setDayOffCount(parseInt(e.target.value))}
                />
            </div>

            <button type="submit">Submit</button>
        </form>
    );
};

export default FinishUserRegistrationForm;
