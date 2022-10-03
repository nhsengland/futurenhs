import Head from 'next/head'

import { LayoutWidthContainer } from '@components/layouts/LayoutWidthContainer'
import { LayoutColumnContainer } from '@components/layouts/LayoutColumnContainer'
import { LayoutColumn } from '@components/layouts/LayoutColumn'
import { WarningCallout } from '@components/generic/WarningCallout'

/**
 * Error 500 page
 */
const Index: (props) => JSX.Element = ({ errors }) => {
    let formattedErrors: string = ''

    if (errors && Object.keys(errors).length > 0) {
        try {
            formattedErrors = JSON.stringify(errors, null, 4)
        } catch (error) {
            console.log('Failed to parse error data')
        }
    }

    return (
        <>
            <Head>
                <title>Unexpected error</title>
                <meta name="description" content="500: Server error" />
            </Head>
            <LayoutWidthContainer>
                <LayoutColumnContainer>
                    <LayoutColumn className="u-py-10">
                        <h1>
                            The FutureNHS website is currently experiencing
                            technical difficulties.
                        </h1>
                        <p className="u-text-lead">
                            We are working to resolve these issues. Please try
                            again later. Thank you for your patience.
                        </p>
                        <p className="u-text-lead">
                            Thank you for your patience.
                        </p>
                        {formattedErrors && (
                            <pre className="u-overflow-x-auto">
                                <WarningCallout
                                    headingLevel={2}
                                    text={{
                                        heading: 'Error data',
                                        body: formattedErrors,
                                    }}
                                    className="u-mb-0"
                                />
                            </pre>
                        )}
                    </LayoutColumn>
                </LayoutColumnContainer>
            </LayoutWidthContainer>
        </>
    )
}

export default Index
