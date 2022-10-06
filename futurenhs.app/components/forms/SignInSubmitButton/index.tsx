import React from 'react'

type Props = {
    submitText: string
    action: string
    csrfToken: string
}

function SignInSubmitButton({ submitText, action, csrfToken }: Props) {
    return (
        <form
            action={action}
            method="POST"
            encType="multipart/form-data"
            className="u-mb-12"
        >
            <input type="hidden" name="csrfToken" value={csrfToken} />
            <input
                type="hidden"
                name="callbackUrl"
                value={process.env.APP_URL}
            />
            <button type="submit" className="c-button">
                {submitText}
            </button>
        </form>
    )
}

export default SignInSubmitButton
