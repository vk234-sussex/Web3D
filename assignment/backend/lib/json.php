<?php
function headersJson() {
    header('Content-Type: application/json; charset=utf-8');
}
function echoJson($data) {
    headersJson();
    echo(json_encode($data));
}