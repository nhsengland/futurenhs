import Head from 'next/head'

import { Link } from '@components/Link'
import { LayoutColumnContainer } from '@components/LayoutColumnContainer'
import { LayoutColumn } from '@components/LayoutColumn'

/**
 * Error 404 page
 */
const Index: (props) => JSX.Element = ({}) => {
    return (
        <>
            <Head>
                <title>Page not found</title>
                <meta name="description" content="404: Page not found" />
            </Head>
            <LayoutColumnContainer>
                <LayoutColumn tablet={8} className="u-py-10">
                    <h1>We can’t find the page you were looking for</h1>
                    <p className="u-text-lead">
                        If you entered a web address please check it was
                        correct.
                    </p>
                    <p className="u-text-lead">
                        You can also browse or search the platform to find the
                        information you need using the search bar or browse the
                        knowledge base for more information.
                    </p>
                    <p className="u-text-lead">
                        Alternatively, return to the{' '}
                        <Link href="/">FutureNHS homepage</Link>.
                    </p>
                </LayoutColumn>
            </LayoutColumnContainer>
        </>
    )
}

export default Index
