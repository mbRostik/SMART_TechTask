import React, { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import userManager from '../../../AuthFiles/authConfig';
import { useAuth } from '../../AuthProvider';
import { ThreeDots } from 'react-loader-spinner';
import config from '../../../config.json';
import './ProjectDetails.css';

const ProjectDetails = () => {
    const { id } = useParams();
    const { user, userData, loading, setLoadingState } = useAuth();
    const [project, setProject] = useState(null);

    const [employeeData, setEmployeeData] = useState({
        userName: '',
        id: id
    });

    const handleChange = (e) => {
        const { name, value } = e.target;
        setEmployeeData({
            ...employeeData,
            [name]: value
        });
    };


    const handleSubmit = async (e) => {
        e.preventDefault();
        setLoadingState(true);
        try {
            const accessToken = await userManager.getUser().then(user => user.access_token);
            const response = await fetch(`${config.apiBaseUrl}/AddEmployeeToTheProject`, {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${accessToken}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(employeeData)
            });
            if (response.status === 200) {
                const fetchProject = async () => {
                    setLoadingState(true);
                    try {
                        const accessToken = await userManager.getUser().then(user => user.access_token);
                        const response = await fetch(`${config.apiBaseUrl}/GetProjectById/${id}`, {
                            method: 'GET',
                            headers: {
                                'Authorization': `Bearer ${accessToken}`,
                                'Content-Type': 'application/json'
                            }
                        });
                        if (!response.ok) throw new Error(`HTTP error! Status: ${response.status}`);
                        const data = await response.json();
                        setProject(data);
                        console.log(project);
                    } catch (error) {
                        console.error('Error fetching project data:', error);
                    } finally {
                        setLoadingState(false);
                    }
                };
                fetchProject();
            }
        } catch (error) {
            console.error('Error adding project', error);
            alert('Failed to add project');
        } finally {
            setLoadingState(false);
        }
    };



    useEffect(() => {
        if (userData) {
            if (userData.position === "PMManager") {
                const fetchProject = async () => {
                    setLoadingState(true);
                    try {
                        const accessToken = await userManager.getUser().then(user => user.access_token);
                        const response = await fetch(`${config.apiBaseUrl}/GetProjectById/${id}`, {
                            method: 'GET',
                            headers: {
                                'Authorization': `Bearer ${accessToken}`,
                                'Content-Type': 'application/json'
                            }
                        });
                        if (!response.ok) throw new Error(`HTTP error! Status: ${response.status}`);
                        const data = await response.json();
                        setProject(data);
                    } catch (error) {
                        console.error('Error fetching project data:', error);
                    } finally {
                        setLoadingState(false);
                    }
                };
                fetchProject();
                console.log(project);

            }
        }
    }, [id, userData]);

    return (
        <div className="project-info-container">
            {loading ? (
                <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh' }}>
                    <ThreeDots color="orange" height={80} width={80} />
                </div>
            ) : (
                    project && (
                    <div>
                    
                            <div className="project-info">
                                <h2>Project Details</h2>
                                <div className="project-detail">
                                    <p><strong>ID:</strong> {project.id}</p>
                                    <p><strong>Start Date:</strong> {project.startDate}</p>
                                    <p><strong>End Date:</strong> {project.endDate}</p>
                                    <p><strong>Project Type:</strong> {project.projectType}</p>
                                    <p><strong>Status:</strong> {project.status}</p>
                                    <p><strong>Project Manager ID:</strong> {project.projectManagerId}</p>
                                    <p><strong>Comment:</strong> {project.comment}</p>
                                </div>
                                <br></br>

                                <h3>Employees:</h3>
                                <div className="employee-list">
                                    {project.employees && project.employees.length > 0 ? (
                                        project.employees.map((employee, index) => (
                                            <div key={index} className="employee-card">
                                                <div className="employee-photo">
                                                    <img
                                                        src={employee.photo ? `data:image/jpeg;base64,${employee.photo}` : "/NoPhoto.jpg"}
                                                        alt={employee.fullName}
                                                        className="EmployeeImage"
                                                    />

                                                </div>
                                                <div className="employee-details">
                                                    <strong> {employee.fullName}</strong>
                                                </div>
                                            </div>
                                        ))
                                    ) : (
                                        <p>No employees assigned to this project.</p>
                                    )}
                                </div>
                            </div>
                            <br></br>
                            <br></br>
                            <br></br>

                     <div className="">
                                {userData && userData.position === 'PMManager' && (
                                    <>
                                        <div className="create-project-form">
                                            <h2>Add employee to the project</h2>
                                            <form onSubmit={handleSubmit}>
                                                <div className="form-group">
                                                    <label>UserName</label>
                                                    <input
                                                        type="text"
                                                        className="form-control"
                                                        name="userName"
                                                        value={employeeData.userName}
                                                        onChange={handleChange}
                                                        required
                                                    />
                                                </div>
                                                <button type="submit" className="save-button">Save Project</button>
                                            </form>
                                        </div>
                                    </>
                                )}
                            </div> 
                    </div>
                    




                )
            )}
        </div>
    );
};

export default ProjectDetails;
