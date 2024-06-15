import React from 'react';
import { useState, useEffect, useRef } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import userManager from '../../../AuthFiles/authConfig';
import { NavLink } from 'react-router-dom';
import { ThreeDots } from 'react-loader-spinner';
import { useAuth } from '../../AuthProvider';
import config from '../../../config.json';
import '../Profile.css'
import './ProfileLeaveRequests.css'

import { ToastContainer, toast } from 'react-toastify';

const ProfileLeaveRequests = () => {
    const [isHovered, setIsHovered] = useState(false);
    const { user, userData, loading, isAuthorized, setLoadingState,
        setIsAuthorizedState,
        setUserState,
        setUserDataState } = useAuth();

    const [sorting, setSorting] = useState({ columnName: '', descending: true });

    const [profileLeaveRequest, setProfileLeaveRequest] = useState(null);
    const [filters, setFilters] = useState({
        absenceReason: '',
        status: '',
    });


    async function fetchLeaveRequestsData() {
        try {
            const accessToken = await userManager.getUser().then(user => user.access_token);
            const requestData = {
                ...sorting,
                ...filters
            };
            const response = await fetch(`${config.apiBaseUrl}/GetSortedUserLeaveRequests`, {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${accessToken}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(requestData)
            });
            if (!response.ok) throw new Error(`HTTP error! Status: ${response.status}`);
            let answer = await response.json();
            setProfileLeaveRequest(answer);
            console.log(profileLeaveRequest);

        } catch (error) {
            console.log('Error while sending the request to the ProjectService');
            return null;
        }
    }
    const handleFilterChange = (e) => {
        const { name, value } = e.target;
        setFilters((prevState) => ({
            ...prevState,
            [name]: value,
        }));
    };

    const handleSort = (column) => {
        setSorting((prevState) => ({
            columnName: column,
            descending: prevState.columnName === column ? !prevState.descending : true,
        }));
    };
    const [leaveRequest, setLeaveRequest] = useState({
        absenceReason: '',
        startDate: '',
        endDate: '',
        comment: ''
    });
    const handleChange = (e) => {
        const { name, value } = e.target;
        setLeaveRequest((prevState) => ({
            ...prevState,
            [name]: value
        }));
    };

    const handleSubmit =async (e) => {
        e.preventDefault();

        setLoadingState(true);
        try {
            const accessToken = await userManager.getUser().then(user => user.access_token);
            console.log(leaveRequest);
            const response = await fetch(`${config.apiBaseUrl}/AddLeaveRequest`, {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${accessToken}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(leaveRequest)
            });
            if (response.status === 200) {
                await fetchLeaveRequestsData();
            }
        } catch (error) {
            console.error('Error adding project', error);
            alert('Failed to add project');
        }
        finally {
            setLoadingState(false);

        }
    };
    useEffect(() => {
        if (userData) {
            const SetData = async () => {
                setLoadingState(true);
                await fetchLeaveRequestsData();
            };
            SetData();
            setLoadingState(false);
        }
    }, [userData, sorting, filters]);

    const formatDate = (dateString) => {
        const date = new Date(dateString);
        return date.toISOString().split('T')[0];
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

                        <div className="">
                            <br></br><br></br><br></br>
                            {userData.peoplePartnerID !== null && userData.peoplePartnerID !== "" &&(
                                <>
                                    <br></br><br></br><br></br>
                                    <div className="leaveRequest-create-form">
                                        <h2>Create Leave Request</h2>
                                        <form onSubmit={handleSubmit}>
                                            <div className="leaveRequest-form-group">
                                                <label>Absence Reason</label>
                                                <select
                                                    name="absenceReason"
                                                    value={leaveRequest.absenceReason}
                                                    onChange={handleChange}
                                                    className="leaveRequest-form-control"
                                                    required
                                                >
                                                    <option value="">Select Absence Reason</option>
                                                    <option value="SickLeave">Sick Leave</option>
                                                    <option value="Vacation">Vacation</option>
                                                    <option value="PersonalLeave">Personal Leave</option>
                                                </select>
                                            </div>
                                            <div className="leaveRequest-form-group">
                                                <label>Start Date</label>
                                                <input
                                                    type="date"
                                                    name="startDate"
                                                    value={leaveRequest.startDate}
                                                    onChange={handleChange}
                                                    className="leaveRequest-form-control"
                                                    required
                                                />
                                            </div>
                                            <div className="leaveRequest-form-group">
                                                <label>End Date</label>
                                                <input
                                                    type="date"
                                                    name="endDate"
                                                    value={leaveRequest.endDate}
                                                    onChange={handleChange}
                                                    className="leaveRequest-form-control"
                                                    required
                                                />
                                            </div>
                                            <div className="leaveRequest-form-group">
                                                <label>Comment</label>
                                                <textarea
                                                    name="comment"
                                                    value={leaveRequest.comment}
                                                    onChange={handleChange}
                                                    className="leaveRequest-form-control"
                                                    rows="4"
                                                />
                                            </div>
                                            <button type="submit" className="save-button">Save</button>
                                        </form>
                                    </div>
                                </>
                            )}
                            <br></br><br></br><br></br>
                        {profileLeaveRequest !== null && (
                            <div className="">
                                <div className="project-filters">
                                    <div className="filter">
                                            <label>Absence Reason</label>
                                            <select name="absenceReason" className="filter-select" onChange={handleFilterChange}>
                                                <option value="">All Absence Reasons</option>
                                                <option value="SickLeave">Sick Leave</option>
                                                <option value="Vacation">Vacation</option>
                                                <option value="PersonalLeave">Personal Leave</option>
                                            </select>
                                        </div>
                                        <div className="filter">
                                            <label>Status</label>
                                            <select name="status" className="filter-select" onChange={handleFilterChange}>
                                                <option value="">All Statuses</option>
                                                <option value="Submitted">Submitted</option>
                                                <option value="Approved">Approved</option>
                                                <option value="Rejected">Rejected</option>
                                                <option value="Canceled">Canceled</option>
                                            </select>
                                        </div>
                                </div>

                                <div className="project-table-container">
                                    <table className="project-table">
                                        <thead className="project-table-header">
                                            <tr>
                                                <th className="sortable-column" onClick={() => handleSort('Id')}>Id</th>
                                                <th className="sortable-column" onClick={() => handleSort('EmployeeId')}>EmployeeId</th>
                                                <th className="sortable-column" onClick={() => handleSort('AbsenceReason')}>AbsenceReason</th>
                                                <th className="sortable-column" onClick={() => handleSort('StartDate')}>Start Date</th>
                                                <th className="sortable-column" onClick={() => handleSort('EndDate')}>End Date</th>
                                                    <th className="sortable-column" onClick={() => handleSort('Comment')}>Comment</th>
                                                    <th className="sortable-column" onClick={() => handleSort('Status')}>Status</th>
                                            </tr>
                                        </thead>
                                        <tbody className="project-table-body">
                                            {profileLeaveRequest.map((leaveRequest, index) => (
                                                <tr key={index} className="project-row">
                                                    <td className="project-cell">{leaveRequest.id}</td>
                                                    <td className="project-cell">{leaveRequest.employeeId}</td>
                                                    <td className="project-cell">{leaveRequest.absenceReason}</td>
                                                    <td className="project-cell">{formatDate(leaveRequest.startDate)}</td>
                                                    <td className="project-cell">
                                                        {leaveRequest.endDate ? formatDate(leaveRequest.endDate) : "None"}
                                                    </td>
                                                    <td className="project-cell">{leaveRequest.comment ? leaveRequest.comment : "None"}</td>
                                                    <td className="project-cell">{leaveRequest.status}</td>
                                                </tr>
                                            ))}
                                        </tbody>
                                    </table>
                                </div>
                            </div>
                        )}



                </div>


            )}
        </div>
    );
};



export default ProfileLeaveRequests;




