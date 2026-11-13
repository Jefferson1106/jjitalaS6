<?php
declare(strict_types=1);

header('Content-Type: application/json; charset=utf-8');
header('Cache-Control: no-store');

function responder(int $estado, array $datos): void
{
    http_response_code($estado);
    echo json_encode($datos, JSON_UNESCAPED_UNICODE | JSON_THROW_ON_ERROR);
    exit;
}

$metodo = $_SERVER['REQUEST_METHOD'] ?? 'GET';
if (!in_array($metodo, ['GET', 'POST', 'PUT', 'DELETE'], true)) {
    header('Allow: GET, POST, PUT, DELETE');
    responder(405, ['error' => 'Método no permitido.']);
}

$datos = $_POST;
if ($metodo === 'PUT' || $metodo === 'DELETE') {
    parse_str(file_get_contents('php://input'), $datos);
}

$codigo = null;
if ($metodo === 'PUT' || $metodo === 'DELETE') {
    $codigo = filter_var($datos['codigo'] ?? null, FILTER_VALIDATE_INT,
        ['options' => ['min_range' => 1]]);
    if ($codigo === false || $codigo === null) {
        responder(400, ['error' => 'El código debe ser un entero positivo.']);
    }
}

if ($metodo === 'POST' || $metodo === 'PUT') {
    $nombre = is_string($datos['nombre'] ?? null) ? trim($datos['nombre']) : '';
    $apellido = is_string($datos['apellido'] ?? null) ? trim($datos['apellido']) : '';
    $edad = filter_var($datos['edad'] ?? null, FILTER_VALIDATE_INT,
        ['options' => ['min_range' => 1, 'max_range' => 120]]);
    if ($nombre === '' || $apellido === '' || $edad === false || $edad === null) {
        responder(400, ['error' => 'Ingrese nombre, apellido y una edad entre 1 y 120 años.']);
    }
    if (preg_match('/^.{1,100}$/us', $nombre) !== 1 ||
        preg_match('/^.{1,100}$/us', $apellido) !== 1) {
        responder(400, ['error' => 'Nombre y apellido deben tener como máximo 100 caracteres.']);
    }
}

try {
    $config = require __DIR__ . '/config.php';
    $conexion = new PDO(
        "mysql:host={$config['host']};port={$config['port']};dbname={$config['database']};charset=utf8mb4",
        $config['username'], $config['password'],
        [PDO::ATTR_ERRMODE => PDO::ERRMODE_EXCEPTION,
         PDO::ATTR_DEFAULT_FETCH_MODE => PDO::FETCH_ASSOC,
         PDO::ATTR_EMULATE_PREPARES => false]
    );

    if ($metodo === 'GET') {
        $estudiantes = $conexion->query('SELECT codigo, nombre, apellido, edad FROM estudiantes ORDER BY codigo')->fetchAll();
        foreach ($estudiantes as &$estudiante) {
            $estudiante['codigo'] = (int) $estudiante['codigo'];
            $estudiante['edad'] = (int) $estudiante['edad'];
        }
        unset($estudiante);
        responder(200, $estudiantes);
    }

    if ($metodo === 'POST') {
        $consulta = $conexion->prepare('INSERT INTO estudiantes (nombre, apellido, edad) VALUES (?, ?, ?)');
        $consulta->execute([$nombre, $apellido, $edad]);
        responder(201, ['mensaje' => 'Estudiante agregado.', 'codigo' => (int) $conexion->lastInsertId()]);
    }

    $conexion->beginTransaction();
    $consulta = $conexion->prepare('SELECT codigo FROM estudiantes WHERE codigo = ? FOR UPDATE');
    $consulta->execute([$codigo]);
    if ($consulta->fetch() === false) {
        $conexion->rollBack();
        responder(404, ['error' => 'El estudiante no existe.']);
    }

    if ($metodo === 'PUT') {
        $consulta = $conexion->prepare('UPDATE estudiantes SET nombre = ?, apellido = ?, edad = ? WHERE codigo = ?');
        $consulta->execute([$nombre, $apellido, $edad, $codigo]);
    } else {
        $consulta = $conexion->prepare('DELETE FROM estudiantes WHERE codigo = ?');
        $consulta->execute([$codigo]);
    }
    $conexion->commit();
    responder(200, ['mensaje' => $metodo === 'PUT' ? 'Estudiante actualizado.' : 'Estudiante eliminado.']);
} catch (Throwable $error) {
    if (isset($conexion) && $conexion->inTransaction()) {
        $conexion->rollBack();
    }
    error_log((string) $error);
    responder(500, ['error' => 'No se pudo completar la operación. Revise la configuración y el registro de PHP.']);
}
