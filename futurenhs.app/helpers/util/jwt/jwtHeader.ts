function jwtHeader(token: string) {
    return { Authorization: `Bearer ${token}` }
}

export default jwtHeader
