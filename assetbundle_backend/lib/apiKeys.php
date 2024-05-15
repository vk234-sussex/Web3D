<?php
function checkKeyValidity() {
    $headers = getallheaders();
    if(isset($headers["api-key"])) {
        return true;
    }
    return false;
}
?>