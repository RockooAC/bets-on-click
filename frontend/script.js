document.addEventListener('DOMContentLoaded', () => {
    const loginModal = document.getElementById("loginModal");
    const registerModal = document.getElementById("registerModal");

    const loginBtn = document.getElementById("loginBtn");
    const registerBtn = document.getElementById("registerBtn");

    const closeButtons = document.getElementsByClassName("close");

    loginBtn.onclick = function() {
        loginModal.style.display = "block";
    }

    registerBtn.onclick = function() {
        registerModal.style.display = "block";
    }

    for (let i = 0; i < closeButtons.length; i++) {
        closeButtons[i].onclick = function() {
            this.parentElement.parentElement.style.display = "none";
        }
    }

    window.onclick = function(event) {
        if (event.target === loginModal || event.target === registerModal) {
            event.target.style.display = "none";
        }
    }

    document.getElementById('loginForm').addEventListener('submit', function(event) {
        event.preventDefault();
        const username = document.getElementById('loginUsername').value;
        const password = document.getElementById('loginPassword').value;
        loginUser(username, password);
    });

    document.getElementById('registerForm').addEventListener('submit', function(event) {
        event.preventDefault();
        const username = document.getElementById('registerUsername').value;
        const email = document.getElementById('registerEmail').value;
        const password = document.getElementById('registerPassword').value;
        registerUser(username, email, password);
    });

    document.getElementById('logoutBtn').addEventListener('click', function() {
        // Usunięcie ciasteczek
        document.cookie = "token=;path=/;expires=Thu, 01 Jan 1970 00:00:00 GMT";
        document.cookie = "role=;path=/;expires=Thu, 01 Jan 1970 00:00:00 GMT";

        localStorage.removeItem('token');
        updateUI(false);
    });

    // Sprawdź, czy użytkownik jest zalogowany i zaktualizuj UI
    const isLoggedIn = localStorage.getItem('token') !== null;
    updateUI(isLoggedIn);
});

document.body.addEventListener('click', function(event) {
    if (event.target.id === 'logoutBtn') {
        localStorage.removeItem('userId');
        localStorage.removeItem('walletId');
        localStorage.removeItem('token');
        localStorage.removeItem('role');
        updateUI(false);
        window.location.href = 'index.html';
    }
});


async function loginUser(Username, Password) {
    try {
        const response = await fetch('http://localhost:4000/users/authenticate', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ Username, Password })
        });
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        const data = await response.json();
        // Ustawienie ciasteczka z tokenem i rolą  Token ważny przez 1 godzinę
        document.cookie = `token=${data.token};path=/;max-age=3600`;
        document.cookie = `role=${data.role};path=/;max-age=3600`;
        localStorage.setItem('userId', data.id);
        localStorage.setItem('token', data.token);
        localStorage.setItem('role', data.role);

        const walletResponse = await fetch(`http://localhost:4000/api/wallet/${data.id}`, {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${localStorage.getItem('token')}`,
                'Content-Type': 'application/json'
            },
        });
        if (!walletResponse.ok) {
            throw new Error(`HTTP error! status: ${walletResponse.status}`);
        }
        const walletData = await walletResponse.json();

        localStorage.setItem('walletId', walletData.id);

        document.getElementById('loginModal').style.display = 'none';
        updateUI(true);
    } catch (error) {
        console.error('Error:', error);
    }

    document.getElementById('loginModal').style.display = 'none';
}

function registerUser(Username, Email, Password) {
    fetch('http://localhost:4000/users/register', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({
            Username,
            Email,
            Password
        })
    })
    .then(response => response.json())
    .then(data => {
        console.log('Registration successful:', data);
    })
    .catch((error) => {
        console.error('Error:', error);
    });

    document.getElementById('registerModal').style.display = 'none';
}

function updateUI(isLoggedIn) {
    const loginBtn = document.getElementById('loginBtn');
    const registerBtn = document.getElementById('registerBtn');
    const links = document.getElementById('link'); 
    const logoutBtn = document.getElementById('logoutBtn');
    const adminPanel = document.getElementById('adminPanel'); 

    // Ukryj wszystkie linki i przyciski
    loginBtn.style.display = 'none';
    registerBtn.style.display = 'none';
    logoutBtn.style.display = 'none';

    
    
    if (adminPanel) adminPanel.style.display = 'none';


    if (isLoggedIn) {
        logoutBtn.style.display = 'block';
        links.style.display = 'flex';
        const role = localStorage.getItem('role');

        
        if (role === 'Admin') {
            if (adminPanel) adminPanel.style.display = 'block';
        }
    } else {
        loginBtn.style.display = 'block';
        registerBtn.style.display = 'block';
        links.style.display = 'none';
    }
}


document.addEventListener('DOMContentLoaded', () => {
    const isLoggedIn = localStorage.getItem('token') !== null;
    updateUI(isLoggedIn);
});

//PANEL ADMINA
async function fetchUsersAndDisplay() {
    if (localStorage.getItem('role') === 'Admin') {
        try {
            const response = await fetch('http://localhost:4000/users', {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${localStorage.getItem('token')}`
                }
            });

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const users = await response.json();
            displayUsers(users);
            
        } catch (error) {
            console.error('Error:', error);
        }
    }
}

async function updateUserRole(userId, role) {
    try {
        const response = await fetch(`http://localhost:4000/users/${userId}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${localStorage.getItem('token')}`
            },
            body: JSON.stringify({ role })
        });

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const data = await response.json();
        console.log(data.message);
        fetchUsersAndDisplay(); // Refresh the user list
    } catch (error) {
        console.error('Error:', error);
    }
}

function displayUsers(users) {
    const table = document.createElement('table');
    table.innerHTML = `
        <tr>
            <th>ID</th>
            <th>Email</th>
            <th>Login</th>
            <th>Role</th>
            <th>Actions</th> <!-- Updated to include Actions -->
        </tr>
    `;

    users.forEach(user => {
        const row = table.insertRow(-1);
        row.innerHTML = `
            <td>${user.id}</td>
            <td>${user.email}</td>
            <td>${user.username}</td>
            <td>${user.role || 'User'}</td>
            <td class="toData">
                <button class="deleteBtn" data-id="${user.id}">Delete</button>
                <button class="grantBtn" data-id="${user.id}">Grant Admin</button>
                <button class="revokeBtn" data-id="${user.id}">Revoke Admin</button>
            </td>
        `;
    });
    const container = document.getElementById('usersContainer');
    container.innerHTML = '';
    container.appendChild(table);
    
    const length = document.getElementsByClassName('deleteBtn').length;
    for (let i = 0; i < length; i++) {
        document.getElementsByClassName('deleteBtn')[i].style.padding = '2.5px';
        document.getElementsByClassName('grantBtn')[i].style.padding = '2.5px';
        document.getElementsByClassName('revokeBtn')[i].style.padding = '2.5px';

        document.getElementsByClassName('deleteBtn')[i].style.marginRight = '5px';
        document.getElementsByClassName('grantBtn')[i].style.marginRight = '5px';
        document.getElementsByClassName('revokeBtn')[i].style.marginRight = '5px';

        document.getElementsByClassName('deleteBtn')[i].style.backgroundColor = '#3498db';
        document.getElementsByClassName('grantBtn')[i].style.backgroundColor = '#2ecc71';
        document.getElementsByClassName('revokeBtn')[i].style.backgroundColor = '#e67e22';

        document.getElementsByClassName('deleteBtn')[i].style.border = 'none';
        document.getElementsByClassName('grantBtn')[i].style.border = 'none';
        document.getElementsByClassName('revokeBtn')[i].style.border = 'none';

        document.getElementsByClassName('deleteBtn')[i].style.borderRadius = '5px';
        document.getElementsByClassName('grantBtn')[i].style.borderRadius = '5px';
        document.getElementsByClassName('revokeBtn')[i].style.borderRadius = '5px';


        document.getElementsByClassName('toData')[i].style.display = 'flex';
        document.getElementsByClassName('toData')[i].style.justifyContent = 'space-between';
    }


    addDeleteHandlers();
    addRoleChangeHandlers();
}

function addRoleChangeHandlers() {
    const grantButtons = document.querySelectorAll('.grantBtn');
    const revokeButtons = document.querySelectorAll('.revokeBtn');

    grantButtons.forEach(button => {
        button.addEventListener('click', function() {
            const userId = this.getAttribute('data-id');
            updateUserRole(userId, 'Admin');
        });
    });

    revokeButtons.forEach(button => {
        button.addEventListener('click', function() {
            const userId = this.getAttribute('data-id');
            updateUserRole(userId, null); // Or 'User' if that's the non-admin role
        });
    });
}



document.addEventListener('DOMContentLoaded', () => {
    fetchUsersAndDisplay();
    showEvents();
    handleEventsDropdown();
    getAllBetId();
    getMoney();
    console.log(document.getElementById('email').value);
    console.log(document.getElementById('name').value);
    console.log(document.getElementById('password').value);
});


function addDeleteHandlers() {
    const deleteButtons = document.querySelectorAll('.deleteBtn');
    deleteButtons.forEach(button => {
        button.addEventListener('click', function() {
            const userId = this.getAttribute('data-id');
            deleteUser(userId);
        });
    });
}

async function deleteUser(userId) {
    try {
        const response = await fetch(`http://localhost:4000/users/${userId}`, {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${localStorage.getItem('token')}`
            }
        });

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        console.log(`User with ID ${userId} has been deleted.`);
        fetchUsersAndDisplay();
    } catch (error) {
        console.error('Error:', error);
    }
}

async function showBets() {
    const userId = localStorage.getItem('userId');
        try {
            const response = await fetch(`http://localhost:4000/users/${userId}/bets`, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${localStorage.getItem('token')}`
                }
            });

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const data = await response.json();
            console.log(data)
        } catch (error) {
            console.error('Error:', error);
        }
}

// Dark/light mode
const toggle = document.getElementById('mode__btn');
toggle.addEventListener('click', function(){
    const html = document.documentElement;
    const wasDarkmode = html.classList.contains('dark-mode');
    toggle.classList.toggle('bi-moon');

    localStorage.setItem('darkmode', !wasDarkmode);
    html.classList.toggle('dark-mode', !wasDarkmode);
})

// Zmiana imienia, nazwiskam email
async function updateUser() {
    const email = document.getElementById('email').value;
    const username = document.getElementById('name').value;
    const password = document.getElementById('password').value;
    const userId = localStorage.getItem('userId');
    try {
        const response = await fetch(`http://localhost:4000/users/${userId}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${localStorage.getItem('token')}`
            }, body: JSON.stringify({
               Email : email,
               Username : username,
               Password : password
            })
        });

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const data = await response.json();
        console.log(data)

    } catch (error) {
        console.error('Error:', error);
    }
}



// Wpłata w portfel
async function putMoney() {
    const amountOfMoney = document.getElementById("amountMoney").value;
    const walletId = localStorage.getItem('walletId');
    try {
        const response = await fetch(`http://localhost:4000/api/wallet/${walletId}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${localStorage.getItem('token')}`
            }, body: JSON.stringify({
                amount: amountOfMoney
            })
        });



        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const data = await response.json();
        console.log(data);
        getMoney();


    } catch (error) {
        console.error('Error:', error);
    }
}

// Wyświetlanie portfela
async function getMoney() {
    const walletId = localStorage.getItem('walletId');
    try {
        const response = await fetch(`http://localhost:4000/api/wallet/${walletId}`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${localStorage.getItem('token')}`
            }
        });



        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const data = await response.json();
        console.log(data.amount);

        document.getElementById("walletBalance").textContent = data.amount;

    } catch (error) {
        console.error('Error:', error);
    }
}

// Dodawanie betów

async function addBets() {
    const betEventDropdown = document.getElementById('betEvent');
    const userId = localStorage.getItem('userId');
    const selectedEventId = betEventDropdown.value;

    console.log('Selected Event ID:', selectedEventId);

    const selectedOption = betEventDropdown.selectedOptions[0].textContent;

    const amount = document.getElementById("betAmount").value;
    console.log(amount);
    try {
        const response = await fetch(`http://localhost:4000/users/${userId}/bets/place`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${localStorage.getItem('token')}`
            }, body: JSON.stringify({
                Amount: amount,
                EventId: selectedEventId
            })
        });


        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const data = await response.json();
        console.log(data);
        getAllBetId();

        const addData = {
            Amount : amount,
            Description : selectedOption
        };

        const betListContainer = document.getElementById('bet');
        const betElement = createBetElement(addData);
        betListContainer.appendChild(betElement);

    } catch (error) {
        console.error('Error:', error);
    }
}

function createBetElement(match) {
    const betContainer = document.createElement('div');
    betContainer.classList.add('bet');

    const betInfo = document.createElement('div');
    betInfo.classList.add('bet-info');
    betInfo.textContent = `${match.Amount} - ${match.Description}`;




    const deleteBtn = document.createElement('button');
    deleteBtn.classList.add('delete-btn');
    deleteBtn.textContent = 'Delete';

    const deleteEventIdPromise = getAllBetId();
    deleteEventIdPromise.then(deleteEventId => {
    deleteBtn.dataset.customValue = deleteEventId;
    console.log(deleteEventId.id);

    deleteBtn.addEventListener('click', () => {
        deleteBet(deleteEventId.id);
        betContainer.remove();
        console.log(`Delete button clicked for bet with ID ${deleteEventId.id}`);
    });
    }).catch(error => {
        console.error('Error:', error);
    });

    betContainer.appendChild(betInfo);
    betContainer.appendChild(deleteBtn);

    return betContainer;
}

async function getAllBetId(){
    const userId = localStorage.getItem('userId');
    try {
        const response = await fetch(`http://localhost:4000/users/${userId}/bets`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${localStorage.getItem('token')}`
            }
        });


        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const data = await response.json();
        return data[data.length - 1];


    } catch (error) {
        console.error('Error:', error);
    }
}



async function elementBetId(){
    try {
        const response = await fetch(`http://localhost:4000/users/1/bets/${deleteEventId}`, { //probka
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${localStorage.getItem('token')}`
            }
        });


        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const data = await response.json();
        // console.log(data);



    } catch (error) {
        console.error('Error:', error);
    }
}

async function deleteBet(deleteEventId){
    try {
        const response = await fetch(`http://localhost:4000/users/1/bets/${deleteEventId}`, { // probka
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${localStorage.getItem('token')}`
            }
        });


        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const data = await response.json();
        //console.log(data);



    } catch (error) {
        console.error('Error:', error);
    }
}

function populateEventsDropdown(events) {
    const betEventDropdown = document.getElementById('betEvent');

    betEventDropdown.innerHTML = '';

    events.forEach(event => {
        const option = document.createElement('option');
        option.id = "optionList";
        option.value = event.id;
        option.text = event.eventName;
        betEventDropdown.appendChild(option);
    });
}

async function handleEventsDropdown(){
    try {
        await fetch('http://localhost:4000/events', {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${localStorage.getItem('token')}`
            }
        })
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            return response.json();
        })
        .then(data => {
            populateEventsDropdown(data);
        })
        .catch(error => {
            console.error('Error:', error);
        });
    } catch (error) {
        console.error('Error:', error);
    }
}



// Wyświetlanie wydarzeń

async function showEvents() {
    try {
        const response = await fetch('http://localhost:4000/events', {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${localStorage.getItem('token')}`
            }
        });

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const data = await response.json();

        const betListContainer = document.getElementById('bet-list');
        betListContainer.innerHTML = '';

        data.forEach(match => {
            const betElement = showEventElement(match);

            betListContainer.appendChild(betElement);
        });


    } catch (error) {
        console.error('Error:', error);
    }
}

function showEventElement(match) {
    const betContainer = document.createElement('div');
    betContainer.classList.add('bet');


    const betInfo = document.createElement('div');
    betInfo.classList.add('bet-info');
    console.log(match);
    betInfo.textContent = `${match.eventName} - ${match.eventStart} - ${match.eventStop}`;

    betContainer.appendChild(betInfo);

    return betContainer;
}



