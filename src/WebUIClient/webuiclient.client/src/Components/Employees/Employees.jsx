import { useEffect, useState } from 'react';
import './Employees.css';
import { Link, useNavigate } from 'react-router-dom';
import userManager from '../../AuthFiles/authConfig';
import { useAuth } from '../AuthProvider';
import { ThreeDots } from 'react-loader-spinner';
import config from '../../config.json';

function Employees() {
    const navigate = useNavigate();
    const { user, userData, loading, isAuthorized, setLoadingState,
        setIsAuthorizedState,
        setUserState,
        setUserDataState } = useAuth();

    const [sorting, setSorting] = useState({ columnName: '', descending: true });

    const [newEmployee, setNewEmployee] = useState({
        fullName: '',
        outOfOfficeBalance: 0,
        peoplePartnerID: '',
        position: 'Employee',
        status: 'Active',
        subdivision: 'HR',
    });

    const handleFormChange = (field, value) => {
        setNewEmployee(prevState => ({
            ...prevState,
            [field]: value
        }));
    };

    const handleSubmit = async (e) => {
        setLoadingState(true);
        e.preventDefault();
        try {
            const accessToken = await userManager.getUser().then(user => user.access_token);
            const response = await fetch(`${config.apiBaseUrl}/CreateEmployee`, {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${accessToken}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(newEmployee)
            });
            if (response.status === 200) {
                await fetchEmployeesData();
            }
        } catch (error) {
            console.error('Error adding employee', error);
            alert('Failed to add employee');
        }
        finally {
            setLoadingState(false);

        }
    };


    const [employeesData, setemployeesData] = useState(null);

    const [filters, setFilters] = useState({
        status: '',
        position: '',
        subdivision: '',
    });
    const handleRowClick = (id) => {
        if (userData && userData.position === 'PMManager' || userData.position === 'Administrator') {
            console.log("fe");
        }
    };
    async function fetchEmployeesData() {
        try {
            const accessToken = await userManager.getUser().then(user => user.access_token);
            const requestData = {
                ...sorting,
                ...filters
            };
            const response = await fetch(`${config.apiBaseUrl}/GetSortedEmployeeTable`, {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${accessToken}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(requestData)
            });

            if (!response.ok) throw new Error(`HTTP error! Status: ${response.status}`);
            let answer = await response.json();
            console.log(answer);

            setemployeesData(answer);
        } catch (error) {
            console.log('Error while sending the request to the UserService');
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
        const newEmployeesData = [...employeesData];
        newEmployeesData[index][field] = value;
        setemployeesData(newEmployeesData);
    };

    useEffect(() => {
        const SetData = async () => {
            setLoadingState(true);
            if (userData.position === "HRManager" || userData.position === "PMManager" || userData.position === 'Administrator') {
                await fetchEmployeesData();
            }
        };
        SetData();
        setLoadingState(false);
    }, [sorting, filters, userData]);

    const handleSave = async (employee) => {
        try {
            setLoadingState(true);
            const accessToken = await userManager.getUser().then(user => user.access_token);
            const response = await fetch(`${config.apiBaseUrl}/ChangeUserInformation`, {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${accessToken}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(employee)
            });
            if (!response.ok) throw new Error(`HTTP error! Status: ${response.status}`);
        } catch (error) {
            console.log('Error while sending the request to the UserService');
        }
        finally {
            await fetchEmployeesData();
            setLoadingState(false);
        }
    };


    return (
        <div className="">
            {loading ? (
                <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh' }}>
                    <ThreeDots color="orange" height={80} width={80} />
                </div>
            ) : !isAuthorized ? (
                <div className="">
                    UnAuth
                </div>
            ) : userData === null ? (
                <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh' }}>
                    <ThreeDots color="#00BFFF" height={80} width={80} />
                </div>
            ) : (
                <div className="EmployeeContainer">
                    {userData && userData.fullyRegistered && employeesData ? (
                        <>
                                        <div className="employee-filters">
                                            <div className="filter">
                                                <label>Status</label>
                                                <select name="status" className="filter-select" onChange={handleFilterChange}>
                                                    <option value="">All Statuses</option>
                                                    <option value="Active">Active</option>
                                                    <option value="Inactive">Inactive</option>
                                                </select>
                                            </div>
                                            <div className="filter">
                                                <label>Position</label>
                                                <select name="position" className="filter-select" onChange={handleFilterChange}>
                                                    <option value="">All Positions</option>
                                                    <option value="Employee">Employee</option>
                                                    <option value="HRManager">HRManager</option>
                                                    <option value="ProjectManager">ProjectManager</option>
                                                    <option value="Administrator">Administrator</option>
                                                </select>
                                            </div>
                                            <div className="filter">
                                                <label>Subdivision</label>
                                                <select name="subdivision" className="filter-select" onChange={handleFilterChange}>
                                                    <option value="">All Subdivisions</option>
                                                    <option value="HR">HR</option>
                                                    <option value="IT">IT</option>
                                                </select>
                                            </div>
                                        </div>

                                        <div className="employee-table-container">
                                            <table className="employee-table">
                                                <thead className="employee-table-header">
                                                    <tr>
                                                        <th className="sortable-column" onClick={() => handleSort('Id')}>Id</th>
                                                        <th className="sortable-column" onClick={() => handleSort('FullName')}>Full Name</th>
                                                        <th className="sortable-column" onClick={() => handleSort('OutOfOfficeBalance')}>Out Of Office Balance</th>
                                                        <th className="sortable-column" onClick={() => handleSort('PeoplePartnerID')}>People Partner ID</th>
                                                        <th className="sortable-column" onClick={() => handleSort('Position')}>Position</th>
                                                        <th className="sortable-column" onClick={() => handleSort('Status')}>Status</th>
                                                        <th className="sortable-column" onClick={() => handleSort('Subdivision')}>Subdivision</th>
                                                        {userData.position === 'HRManager' || userData.position === 'Administrator' && <th className="actions-column">Actions</th>}
                                                    </tr>
                                                </thead>
                                                <tbody className="employee-table-body">
                                                    {employeesData.map((employee, index) => (
                                                        <tr key={index} className="employee-row">
                                                            <td className="employee-cell">{employee.id}</td>
                                                            <td className="employee-cell" onClick={() => handleRowClick(employee.id)}>
                                                                {userData.position === 'HRManager' || userData.position === 'Administrator' ? (
                                                                    <input
                                                                        type="text"
                                                                        className="employee-input"
                                                                        value={employee.fullName}
                                                                        onChange={(e) => handleInputChange(index, 'fullName', e.target.value)}
                                                                    />
                                                                ) : (
                                                                    employee.fullName
                                                                )}
                                                            </td>
                                                            <td className="employee-cell">
                                                                {userData.position === 'HRManager' || userData.position === 'Administrator' ? (
                                                                    <input
                                                                        type="number"
                                                                        className="employee-input"
                                                                        value={employee.outOfOfficeBalance}
                                                                        onChange={(e) => handleInputChange(index, 'outOfOfficeBalance', e.target.value)}
                                                                    />
                                                                ) : (
                                                                    employee.outOfOfficeBalance
                                                                )}
                                                            </td>
                                                            <td className="employee-cell">
                                                                {userData.position === 'HRManager' || userData.position === 'Administrator' ? (
                                                                    <input
                                                                        type="text"
                                                                        className="employee-input"
                                                                        value={employee.peoplePartnerID || ""}
                                                                        onChange={(e) => handleInputChange(index, 'peoplePartnerID', e.target.value)}
                                                                    />
                                                                ) : (
                                                                    employee.peoplePartnerID ? employee.peoplePartnerID : "None"
                                                                )}
                                                            </td>
                                                            <td className="employee-cell">
                                                                {userData.position === 'HRManager' || userData.position === 'Administrator' ? (
                                                                    <select
                                                                        className="employee-select"
                                                                        value={employee.position}
                                                                        onChange={(e) => handleInputChange(index, 'position', e.target.value)}
                                                                    >
                                                                        <option value="Employee">Employee</option>
                                                                        <option value="HRManager">HRManager</option>
                                                                        <option value="PMManager">ProjectManager</option>
                                                                        <option value="Administrator">Administrator</option>
                                                                    </select>
                                                                ) : (
                                                                    employee.position
                                                                )}
                                                            </td>
                                                            <td className="employee-cell">
                                                                {userData.position === 'HRManager' || userData.position === 'Administrator' ? (
                                                                    <select
                                                                        className="employee-select"
                                                                        value={employee.status}
                                                                        onChange={(e) => handleInputChange(index, 'status', e.target.value)}
                                                                    >
                                                                        <option value="Active">Active</option>
                                                                        <option value="Inactive">Inactive</option>
                                                                    </select>
                                                                ) : (
                                                                    employee.status
                                                                )}
                                                            </td>
                                                            <td className="employee-cell">
                                                                {userData.position === 'HRManager' || userData.position === 'Administrator' ? (
                                                                    <select
                                                                        className="employee-select"
                                                                        value={employee.subdivision}
                                                                        onChange={(e) => handleInputChange(index, 'subdivision', e.target.value)}
                                                                    >
                                                                        <option value="HR">HR</option>
                                                                        <option value="IT">IT</option>
                                                                    </select>
                                                                ) : (
                                                                    employee.subdivision
                                                                )}
                                                            </td>
                                                            {userData.position === 'HRManager' || userData.position === 'Administrator' && (
                                                                <td className="employee-cell">
                                                                    <button className="save-button" onClick={() => handleSave(employee)}>Save</button>
                                                                </td>
                                                            )}
                                                        </tr>
                                                    ))}
                                                </tbody>
                                            </table>
                                        </div>


                                        <div className="">
                                            {userData && userData.position === 'HRManager' && (
                                                <>
                                                    <div className="create-employee-form">
                                                    <h2>Create New Employee</h2>
                                                    <form>
                                                        <div className="create-employee-form-sides">
                                                            <div className="form-group-left">
                                                                <div className="form-group">
                                                                    <label>Full Name</label>
                                                                    <input
                                                                        type="text"
                                                                        value={newEmployee.fullName}
                                                                            onChange={(e) => handleFormChange('fullName', e.target.value)}
                                                                        className="form-control"
                                                                    />
                                                                </div>
                                                                <div className="form-group">
                                                                    <label>Out Of Office Balance</label>
                                                                    <input
                                                                        type="number"
                                                                        value={newEmployee.outOfOfficeBalance}
                                                                            onChange={(e) => handleFormChange('outOfOfficeBalance', e.target.value)}
                                                                        className="form-control"
                                                                    />
                                                                </div>
                                                                <div className="form-group">
                                                                    <label>People Partner ID</label>
                                                                    <input
                                                                        type="text"
                                                                        value={newEmployee.peoplePartnerID}
                                                                            onChange={(e) => handleFormChange('peoplePartnerID', e.target.value)}
                                                                        className="form-control"
                                                                    />
                                                                </div>
                                                            </div>
                                                            <div className="form-group-right">
                                                                <div className="form-group">
                                                                    <label>Position</label>
                                                                    <select
                                                                        value={newEmployee.position}
                                                                            onChange={(e) => handleFormChange('position', e.target.value)}
                                                                        className="form-control"
                                                                    >
                                                                        <option value="Employee">Employee</option>
                                                                        <option value="HRManager">HRManager</option>
                                                                        <option value="PMManager">ProjectManager</option>
                                                                        <option value="Administrator">Administrator</option>
                                                                    </select>
                                                                </div>
                                                                <div className="form-group">
                                                                    <label>Status</label>
                                                                    <select
                                                                        value={newEmployee.status}
                                                                            onChange={(e) => handleFormChange('status', e.target.value)}
                                                                        className="form-control"
                                                                    >
                                                                        <option value="Active">Active</option>
                                                                        <option value="Inactive">Inactive</option>
                                                                    </select>
                                                                </div>
                                                                <div className="form-group">
                                                                    <label>Subdivision</label>
                                                                    <select
                                                                        value={newEmployee.subdivision}
                                                                            onChange={(e) => handleFormChange('subdivision', e.target.value)}
                                                                        className="form-control"
                                                                    >
                                                                        <option value="HR">HR</option>
                                                                        <option value="IT">IT</option>
                                                                    </select>
                                                                </div>
                                                            </div>
                                                        </div>
                                                       
                                                        
                                                            <button type="button" onClick={handleSubmit} className="save-button">Save</button>
                                                        </form>
                                                    </div> 
                                                </>
                                            )}
                                        </div> 

                        </>
                    ) : (
                        <div className="">Loading...</div>
                    )}
                </div>
            )}
        </div>
    );
}

export default Employees;