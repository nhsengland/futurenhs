export const getLastMonthDate = () => {
    const today = new Date()
    today.setMonth(today.getMonth() - 1)
    const yyyy = today.getFullYear()
    let mm = today.getMonth() + 1 // Months start at 0!
    let dd = today.getDate()

    const dateString = `${dd < 10 ? '0' : ''}${dd}/${
        mm < 10 ? '0' : ''
    }${mm}/${yyyy}`
    return dateString
}

export const getDateFromUTC = (utc: string) => {
    const date = new Date(utc)
    const yyyy = date.getFullYear()
    let mm = date.getMonth() + 1 // Months start at 0!
    let dd = date.getDate()

    const dateString = `${dd < 10 ? '0' : ''}${dd}/${
        mm < 10 ? '0' : ''
    }${mm}/${yyyy}`
    return dateString
}
