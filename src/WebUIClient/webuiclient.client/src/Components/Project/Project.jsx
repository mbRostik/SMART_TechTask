import { useEffect, useState } from 'react';
import './Project.css';
import { Link, useNavigate } from 'react-router-dom';
import userManager from '../../AuthFiles/authConfig';
import { useAuth } from '../AuthProvider';
import { ThreeDots } from 'react-loader-spinner';
import config from '../../config.json';

function Project() {
    const navigate = useNavigate();
    const { user, userData, loading, isAuthorized, setLoadingState,
        setIsAuthorizedState,
        setUserState,
        setUserDataState } = useAuth();

    const [sorting, setSorting] = useState({ columnName: '', descending: true });

    const [projectsData, setProjectsData] = useState(null);

    const [filters, setFilters] = useState({
        projectType: '',
        status: '',
    });

    const [projectData, setProjectData] = useState({
        projectType: '',
        startDate: '',
        endDate: '',
        projectManagerId: '',
        comment: '',
        status: ''
    });


    const handleRowClick = (id) => {
        if (userData && userData.position === 'PMManager') {
            console.log("Redirecting to project details");
        }
    };

    async function fetchProjectsData() {
        try {
            const accessToken = await userManager.getUser().then(user => user.access_token);
            const requestData = {
                ...sorting,
                ...filters
            };
            const response = await fetch(`${config.apiBaseUrl}/GetSortedProjectTable`, {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${accessToken}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(requestData)
            });
            if (!response.ok) throw new Error(`HTTP error! Status: ${response.status}`);
            let answer = await response.json();
            setProjectsData(answer);
            console.log(projectsData);

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

    const handleInputChange = (index, field, value) => {
        const newProjectsData = [...projectsData];
        newProjectsData[index][field] = value;
        setProjectsData(newProjectsData);
    };



    const handleChange = (e) => {
        const { name, value } = e.target;
        setProjectData({
            ...projectData,
            [name]: value
        });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setLoadingState(true);
        try {
            const accessToken = await userManager.getUser().then(user => user.access_token);
            const response = await fetch(`${config.apiBaseUrl}/CreateProject`, {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${accessToken}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(projectData)
            });
            if (response.status === 200) {
                await fetchProjectsData();
            }
        } catch (error) {
            console.error('Error adding project', error);
            alert('Failed to add project');
        }
        finally {
            setLoadingState(false);

        }
    };

    const handleProjectClick = (id) => {
        window.location.href = `/ProjectDetails/${id}`;
    };
    useEffect(() => {
        
        const SetData = async () => {
            setLoadingState(true);
            if (userData.position === "PMManager" || userData.position === "HRManager") {
                await fetchProjectsData();
            }
        };
        SetData();
        setLoadingState(false);
    }, [sorting, filters, userData]);

    const handleSave = async (project) => {
        try {
            setLoadingState(true);
            const accessToken = await userManager.getUser().then(user => user.access_token);
            const response = await fetch(`${config.apiBaseUrl}/ChangeProject`, {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${accessToken}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(project)
            });
            if (!response.ok) throw new Error(`HTTP error! Status: ${response.status}`);
            alert("Changes saved successfully");
        } catch (error) {
            console.log('Error while sending the request to the ProjectService');
        }
        finally {
            setLoadingState(false);
        }
    };
    const formatDate = (dateString) => {
        const date = new Date(dateString);
        return date.toISOString().split('T')[0]; 
    };
    return (
        <div className="">
            {loading ? (
                <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh' }}>
                    <ThreeDots color="orange" height={80} width={80} />
                </div>
            ) : isAuthorized === false ? (
                <div className="">
                    UnAuth
                </div>
            ) : userData === null ? (
                <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh' }}>
                    <ThreeDots color="#00BFFF" height={80} width={80} />
                </div>
            ) : (
                <div className="EmployeeContainer">
                    {userData && userData.fullyRegistered && projectsData ? (
                        <>
                            <div className="project-filters">
                                <div className="filter">
                                    <label>Project Type</label>
                                    <select name="projectType" className="filter-select" onChange={handleFilterChange}>
                                        <option value="">All Types</option>
                                        <option value="Internal">Internal</option>
                                        <option value="External">External</option>
                                    </select>
                                </div>
                                <div className="filter">
                                    <label>Status</label>
                                    <select name="status" className="filter-select" onChange={handleFilterChange}>
                                        <option value="">All Statuses</option>
                                        <option value="Active">Active</option>
                                        <option value="Inactive">Inactive</option>
                                    </select>
                                </div>
                            </div>

                            <div className="project-table-container">
                                <table className="project-table">
                                    <thead className="project-table-header">
                                        <tr>
                                            <th className="sortable-column" onClick={() => handleSort('Id')}>Id</th>
                                            <th className="sortable-column" onClick={() => handleSort('ProjectType')}>Project Type</th>
                                            <th className="sortable-column" onClick={() => handleSort('Status')}>Status</th>
                                            <th className="sortable-column" onClick={() => handleSort('StartDate')}>Start Date</th>
                                            <th className="sortable-column" onClick={() => handleSort('EndDate')}>End Date</th>
                                            <th className="sortable-column" onClick={() => handleSort('ProjectManagerId')}>Project Manager ID</th>
                                            <th className="sortable-column" onClick={() => handleSort('Comment')}>Comment</th>
                                            {userData.position === 'PMManager' && <th className="actions-column">Actions</th>}
                                        </tr>
                                    </thead>
                                    <tbody className="project-table-body">
                                        {projectsData.map((project, index) => (
                                            <tr key={index} className="project-row">
                                                <td onClick={() => handleProjectClick(project.id)} className="project-cell">{project.id}</td>
                                                <td className="project-cell" onClick={() => handleRowClick(project.id)}>
                                                    {userData.position === 'PMManager' ? (
                                                        <select
                                                            className="project-select"
                                                            value={project.projectType}
                                                            onChange={(e) => handleInputChange(index, 'projectType', e.target.value)}
                                                        >
                                                            <option value="Internal">Internal</option>
                                                            <option value="External">External</option>
                                                        </select>
                                                    ) : (
                                                        project.projectType
                                                    )}
                                                </td>

                                                <td className="project-cell" onClick={() => handleRowClick(project.id)}>
                                                    {userData.position === 'PMManager' ? (
                                                        <select
                                                            className="project-select"
                                                            value={project.status}
                                                            onChange={(e) => handleInputChange(index, 'status', e.target.value)}
                                                        >
                                                            <option value="Active">Active</option>
                                                            <option value="Inactive">Inactive</option>
                                                        </select>
                                                    ) : (
                                                        project.status
                                                    )}
                                                </td>

                                                <td className="project-cell">
                                                    {userData.position === 'PMManager' ? (
                                                        <input
                                                            type="date"
                                                            className="project-input"
                                                            value={formatDate(project.startDate)}
                                                            onChange={(e) => handleInputChange(index, 'startDate', e.target.value)}
                                                        />
                                                    ) : (
                                                        project.startDate
                                                    )}
                                                </td>
                                                <td className="project-cell">
                                                    {userData.position === 'PMManager' ? (
                                                        <input
                                                            type="date"
                                                            className="project-input"
                                                            value={project.endDate ? formatDate(project.endDate) : ""}
                                                            onChange={(e) => handleInputChange(index, 'endDate', e.target.value)}
                                                        />
                                                    ) : (
                                                        project.endDate ? project.endDate : "None"
                                                    )}
                                                </td>
                                                <td className="project-cell">
                                                    {userData.position === 'PMManager' ? (
                                                        <input
                                                            type="text"
                                                            className="project-input"
                                                            value={project.projectManagerId}
                                                            onChange={(e) => handleInputChange(index, 'projectManagerId', e.target.value)}
                                                        />
                                                    ) : (
                                                        project.projectManagerId
                                                    )}
                                                </td>
                                                <td className="project-cell">
                                                    {userData.position === 'PMManager' ? (
                                                        <input
                                                            type="text"
                                                            className="project-input"
                                                            value={project.comment || ""}
                                                            onChange={(e) => handleInputChange(index, 'comment', e.target.value)}
                                                        />
                                                    ) : (
                                                        project.comment ? project.comment : "None"
                                                    )}
                                                </td>
                                                {userData.position === 'PMManager' && (
                                                    <td className="project-cell">
                                                        <button className="save-button" onClick={() => handleSave(project)}>Save</button>
                                                    </td>
                                                )}
                                            </tr>
                                        ))}
                                    </tbody>
                                </table>
                                        </div>

                            <div className="">
                                {userData && userData.position === 'PMManager' && (
                                    <>
                                        <div className="create-project-form">
                                            <h2>Create New Project</h2>
                                            <form onSubmit={handleSubmit}>



                                                <div className="form-group">
                                                    <label>Project Type</label>
                                                    <select
                                                        className="form-control"
                                                        name="projectType"
                                                        value={projectData.projectType}
                                                        onChange={handleChange}
                                                        required
                                                                >
                                                    <option value="">Select Project Type</option>

                                                    <option value="Internal">Internal</option>
                                                    <option value="External">External</option>
                                                    </select>
                                                </div>

                                                <div className="form-group">
                                                    <label>Start Date</label>
                                                    <input
                                                        type="date"
                                                        className="form-control"
                                                        name="startDate"
                                                        value={projectData.startDate}
                                                        onChange={handleChange}
                                                        required
                                                    />
                                                </div>
                                                <div className="form-group">
                                                    <label>End Date</label>
                                                    <input
                                                        type="date"
                                                        className="form-control"
                                                        name="endDate"
                                                        value={projectData.endDate}
                                                        onChange={handleChange}
                                                    />
                                                </div>
                                                <div className="form-group">
                                                    <label>Project Manager</label>
                                                    <input
                                                        type="text"
                                                        className="form-control"
                                                        name="projectManagerId"
                                                        value={projectData.projectManagerId}
                                                        onChange={handleChange}
                                                        required
                                                    />
                                                </div>
                                                <div className="form-group">
                                                    <label>Comment</label>
                                                    <textarea
                                                        className="form-control"
                                                        name="comment"
                                                        value={projectData.comment}
                                                        onChange={handleChange}
                                                    />
                                                </div>
                                                <div className="form-group">
                                                    <label>Status</label>
                                                    <select
                                                        className="form-control"
                                                        name="status"
                                                        value={projectData.status}
                                                        onChange={handleChange}
                                                        required
                                                    >
                                                        <option value="">Select Status</option>
                                                        <option value="Active">Active</option>
                                                        <option value="Inactive">Inactive</option>
                                                    </select>
                                                </div>
                                                <button type="submit" className="save-button">Save Project</button>
                                            </form>
                                        </div>
                                    </>
                                )}
                            </div> 
                        </>
                    ) : (
                        <div className="">
                            Loading...
                        </div>
                    )}
                </div>
            )}
        </div>
    );
}

export default Project;
