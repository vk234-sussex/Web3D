<?php
include_once 'lib.php';
$key = $_GET["key"];

class MyDB extends SQLite3
{
    function __construct()
    {
        $this->open('storage.db');
        if (!$this) {
         die($this->lastErrorMsg());
        }
    }

    function lookupWithKey($key) {
      $statement = $this->prepare('SELECT * FROM "elements" WHERE "key" = ?');
      $statement->bindValue(1, $key);
      $result = $statement->execute();
      return $result;
    }
}
$db = new MyDB();
$result = $db->lookupWithKey($_GET["key"]);
$data = $result->fetchArray(SQLITE3_ASSOC);
$result->finalize() or die("failed to finalise!");
$db->close();
echoJson($data);