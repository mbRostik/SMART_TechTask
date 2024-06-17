import { useEffect, useState } from 'react';
import './ProfileProject.css';
import { Link, useNavigate } from 'react-router-dom';
import userManager from '../../../AuthFiles/authConfig';
import { useAuth } from '../../AuthProvider';
import { ThreeDots } from 'react-loader-spinner';
import config from '../../../config.json';

function ProfileProject() {
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
            const response = await fetch(`${config.apiBaseUrl}/GetSortedUserProjects`, {
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


    const handleProjectClick = (id) => {
        window.location.href = `/ProjectDetails/${id}`;
    };
    useEffect(() => {
        
        const SetData = async () => {
            setLoadingState(true);
            await fetchProjectsData();
        };
        SetData();
        setLoadingState(false);
    }, [sorting, filters, userData]);


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
                                        </tr>
                                    </thead>
                                                <tbody className="project-table-body">
                                                    {projectsData.map((project, index) => (
                                                        <tr key={index} className="project-row">
                                                            <td onClick={() => handleProjectClick(project.id)} className="project-cell">{project.id}</td>
                                                            <td className="project-cell">{project.projectType}</td>
                                                            <td className="project-cell">{project.status}</td>
                                                            <td className="project-cell">{formatDate(project.startDate)}</td>
                                                            <td className="project-cell">
                                                                {project.endDate ? formatDate(project.endDate) : "None"}
                                                            </td>
                                                            <td className="project-cell">{project.projectManagerId}</td>
                                                            <td className="project-cell">{project.comment ? project.comment : "None"}</td>
                                                        </tr>
                                                    ))}
                                                </tbody>

                                </table>
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

export default ProfileProject;
