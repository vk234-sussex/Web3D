<?php
/* Allow Unity to read all our APIs */
function cors() {
    header("Access-Control-Allow-Origin: *");
    header('Access-Control-Allow-Credentials: true');
    header('Access-Control-Max-Age: 86400');
    header('Access-Control-Allow-Methods: GET, PUT, POST, DELETE, HEAD');
    header('Access-Control-Allow-Headers: origin, content-type, accept, cache-control, pragma, user-agent');
}