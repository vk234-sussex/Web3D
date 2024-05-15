<?php
function echoJson($data) {
    header('Content-Type: application/json; charset=utf-8');
    echo(json_encode($data));
}
?>