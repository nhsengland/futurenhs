import Head from 'next/head'

import { Props } from './interfaces'

const DocumentHead: (props: Props) => JSX.Element = ({ assetPath = '' }) => {
    return (
        <Head>
            <link
                rel="apple-touch-icon"
                sizes="180x180"
                href={`${assetPath}/favicon/favicon/apple-touch-icon.png`}
            />
            <link
                rel="icon"
                type="image/png"
                sizes="32x32"
                href={`${assetPath}/favicon/favicon-32x32.png`}
            />
            <link
                rel="icon"
                type="image/png"
                sizes="16x16"
                href={`${assetPath}/favicon/favicon-16x16.png`}
            />
            <link rel="manifest" href={`${assetPath}/favicon/manifest.json`} />
            <link
                rel="mask-icon"
                href={`${assetPath}/favicon/safari-pinned-tab.svg`}
                color="#000000"
            />
            <link
                rel="shortcut icon"
                href={`${assetPath}/favicon/favicon.ico`}
            />
            <meta name="msapplication-TileColor" content="#000000" />
            <meta
                name="msapplication-config"
                content={`${assetPath}/favicon/browserconfig.xml`}
            />
            <meta name="theme-color" content="#000" />
            <link
                rel="alternate"
                type="application/rss+xml"
                href={`${assetPath}/favicon/feed.xml`}
            />
            <meta name="description" content={``} />
        </Head>
    )
}

export default DocumentHead
