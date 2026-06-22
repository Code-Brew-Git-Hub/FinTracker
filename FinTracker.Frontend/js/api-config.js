const API_PORT = 5009;

const API_URL =
    window.location.protocol === "file:" || !window.location.hostname
        ? `http://localhost:${API_PORT}/api`
        : `${window.location.protocol}//${window.location.hostname}:${API_PORT}/api`;
