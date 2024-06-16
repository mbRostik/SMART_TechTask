import React from 'react';
import { useState, useEffect, useRef } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import userManager from '../../AuthFiles/authConfig';
import { NavLink } from 'react-router-dom';
import { ThreeDots } from 'react-loader-spinner';
import { useAuth } from '../AuthProvider';
import config from '../../config.json';
import '../Profile/Profile.css'
import './ApprovalRequest.css'

import '../Profile/ProfileLeaveRequests/ProfileLeaveRequests.css'

import { ToastContainer, toast } from 'react-toastify';

const ApprovalRequest = () => {
    const [isHovered, setIsHovered] = useState(false);
    const { user, userData, loading, isAuthorized, setLoadingState,
        setIsAuthorizedState,
        setUserState,
        setUserDataState } = useAuth();

    const [sorting, setSorting] = useState({ columnName: '', descending: true });

    const [profileApprovalRequest, setProfileApprovalRequest] = useState(null);
    const [filters, setFilters] = useState({
        status: '',
    });


    async function fetchLeaveRequestsData() {
        try {
            const accessToken = await userManager.getUser().then(user => user.access_token);
            const requestData = {
                ...sorting,
                ...filters
            };
            const response = await fetch(`${config.apiBaseUrl}/GetSortedApprovalRequests`, {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${accessToken}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(requestData)
            });
            if (!response.ok) throw new Error(`HTTP error! Status: ${response.status}`);
            let answer = await response.json();

            setProfileApprovalRequest(answer);
            console.log(answer);

        } catch (error) {
            console.log('Error while sending the request to the ProjectService');
            return null;
        }
    }

    const handleSave = async (leaveRequest) => {
        setLoadingState(true);
        try {
            const accessToken = await userManager.getUser().then(user => user.access_token);
            const changeApprovalRequestDTO = {
                id: leaveRequest.id,
                status: leaveRequest.status,
                comment: leaveRequest.comment
            };
            const response = await fetch(`${config.apiBaseUrl}/ChangeApprovalRequest`, {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${accessToken}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(changeApprovalRequestDTO)
            });
            if (!response.ok) throw new Error(`HTTP error! Status: ${response.status}`);
            await fetchLeaveRequestsData();
        } catch (error) {
            console.error('Error saving changes', error);
        } finally {
            setLoadingState(false);
        }
    };


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


    useEffect(() => {
        if (userData && (userData.position === "PMManager" || userData.position === "HRManager" || userData.position === 'Administrator')) {
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
    const handleRowClick = (leaveRequest) => {
        setSelectedLeaveRequest(leaveRequest.giveLeaveRequestDTO);
    };
    const [selectedLeaveRequest, setSelectedLeaveRequest] = useState(null);

    const handleCloseDetails = () => {
        setSelectedLeaveRequest(null);
    };

    const handleInputChange = (index, field, value) => {
        const newLeaveRequests = [...profileApprovalRequest];
        newLeaveRequests[index][field] = value;
        setProfileApprovalRequest(newLeaveRequests);
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
                    <br></br>
                            
                            {profileApprovalRequest !== null && profileApprovalRequest && (
                                <div className="profileApprovalRequest_Main">

                                    <div className="profileApprovalRequest_Main_Left">
                                        <div className="leaveRequest-filters">
                                            <div className="leaveRequest-filter">
                                                <label>Status</label>
                                                <select name="status" className="leaveRequest-filter-select" onChange={handleFilterChange}>
                                                    <option value="">All Statuses</option>
                                                    <option value="New">New</option>
                                                    <option value="Approved">Approved</option>
                                                    <option value="Rejected">Rejected</option>
                                                    <option value="Canceled">Canceled</option>
                                                </select>
                                            </div>
                                        </div>

                                        <div className="leaveRequest-table-container">
                                            <table className="leaveRequest-table">
                                                <thead className="leaveRequest-table-header">
                                                    <tr>
                                                        <th className="leaveRequest-sortable-column" onClick={() => handleSort('Id')}>Id</th>
                                                        <th className="leaveRequest-sortable-column" onClick={() => handleSort('LeaveRequestId')}>LeaveRequestId</th>
                                                        <th className="leaveRequest-sortable-column" onClick={() => handleSort('Comment')}>Comment</th>
                                                        <th className="leaveRequest-sortable-column" onClick={() => handleSort('Status')}>Status</th>
                                                        <th className="leaveRequest-sortable-column">Actions</th>
                                                    </tr>
                                                </thead>
                                                <tbody className="leaveRequest-table-body">
                                                    {profileApprovalRequest.map((leaveRequest, index) => (
                                                        <tr key={index} className="leaveRequest-row" onClick={() => handleRowClick(leaveRequest)}>
                                                            <td className="leaveRequest-cell">{leaveRequest.id}</td>
                                                            <td className="leaveRequest-cell">{leaveRequest.leaveRequestId}</td>
                                                            <td className="leaveRequest-cell">
                                                                <input
                                                                    type="text"
                                                                    value={leaveRequest.comment || ''} 
                                                                    onChange={(e) => handleInputChange(index, 'comment', e.target.value)}
                                                                    className="leaveRequest-input"
                                                                />
                                                            </td>
                                                            <td className="leaveRequest-cell">
                                                                <select
                                                                    value={leaveRequest.status}
                                                                    onChange={(e) => handleInputChange(index, 'status', e.target.value)}
                                                                    className="leaveRequest-select"
                                                                >
                                                                    <option value="New">New</option>
                                                                    <option value="Approved">Approved</option>
                                                                    <option value="Rejected">Rejected</option>
                                                                    <option value="Canceled">Canceled</option>
                                                                </select>
                                                            </td>
                                                            <td className="leaveRequest-cell">
                                                                <button className="leaveRequest-save-button" onClick={() => handleSave(leaveRequest)}>Save</button>
                                                            </td>
                                                        </tr>
                                                    ))}
                                                </tbody>
                                            </table>
                                        </div>
                                    </div>

                                    <div className="profileApprovalRequest_Main_Right">
                                        {selectedLeaveRequest && (
                                            <div className="leaveRequest-details">
                                                <h3>Leave Request Details</h3>
                                                <p><strong>ID:</strong> {selectedLeaveRequest.id}</p>
                                                <p><strong>Employee ID:</strong> {selectedLeaveRequest.employeeId}</p>
                                                <p><strong>Absence Reason:</strong> {selectedLeaveRequest.absenceReason}</p>
                                                <p><strong>Start Date:</strong> {formatDate(selectedLeaveRequest.startDate)}</p>
                                                <p><strong>End Date:</strong> {selectedLeaveRequest.endDate ? formatDate(selectedLeaveRequest.endDate) : "None"}</p>
                                                <p><strong>Comment:</strong> {selectedLeaveRequest.comment ? selectedLeaveRequest.comment : "None"}</p>
                                                <p><strong>Status:</strong> {selectedLeaveRequest.status}</p>
                                                <button className="leaveRequest-close-button" onClick={handleCloseDetails}>Close</button>
                                            </div>
                                        )}
                                    </div>
                                </div>
                            )}




                </div>


            )}
        </div>
    );
};



export default ApprovalRequest;




