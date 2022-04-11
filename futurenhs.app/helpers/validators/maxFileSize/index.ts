export const maxFileSize = (validationMethodData: any): Function => {
    return (files: any): string => {
        try {
            const message: string = validationMethodData.message
            const maxFileSize: number = validationMethodData.maxFileSize

            let valid: boolean = true

            if (
                files &&
                files === Object(files) &&
                files.length &&
                maxFileSize
            ) {
                for (let i = 0; i < files.length; i++) {
                    const file: any = files[i]
                    const { size } = file

                    if (size && size > maxFileSize) {
                        valid = false
                        break
                    }
                }
            }

            return valid ? undefined : message
        } catch (error) {
            return 'An unexpected error occured'
        }
    }
}
